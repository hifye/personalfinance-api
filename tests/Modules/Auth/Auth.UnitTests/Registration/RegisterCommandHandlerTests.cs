using Auth.Application.Abstractions.Authentication;
using Auth.Application.Abstractions.Persistance;
using Auth.Application.Registration;
using Auth.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;
using SharedKernel.ValueObjects;
using Xunit;

namespace Auth.UnitTests.Registration;

public class RegisterCommandHandlerTests
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IPasswordHasher _passwordHasherMock;
    private readonly ILogger<RegisterCommandHandler> _loggerMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _passwordHasherMock = Substitute.For<IPasswordHasher>();
        _loggerMock = Substitute.For<ILogger<RegisterCommandHandler>>();

        _handler = new RegisterCommandHandler(
            _userRepositoryMock,
            _unitOfWorkMock,
            _passwordHasherMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenEmailIsInvalid()
    {
        // Arrange
        var command = new RegisterCommand("Test User", "invalid-email", "password123", "password123");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Invalid Email");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenEmailAlreadyExists()
    {
        // Arrange
        var command = new RegisterCommand("Test User", "test@example.com", "password123", "password123");
        var email = Email.Create(command.Email).Value;
        
        _userRepositoryMock.GetUserByEmail(email!).Returns(User.Create("Existing", command.Email, "hash").Value);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Email already exists");
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var command = new RegisterCommand("Test User", "test@example.com", "password123", "password123");
        var email = Email.Create(command.Email).Value;

        _userRepositoryMock.GetUserByEmail(email!).Returns((User?)null);
        _passwordHasherMock.HashPassword(command.Password).Returns("hashed_password");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _userRepositoryMock.Received(1).CreateUser(Arg.Is<User>(u => u.Email.Address == command.Email));
        await _unitOfWorkMock.Received(1).CommitAsync();
    }
}
