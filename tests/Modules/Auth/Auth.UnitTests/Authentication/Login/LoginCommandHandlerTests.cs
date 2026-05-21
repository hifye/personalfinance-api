using Auth.Application.Abstractions.Authentication;
using Auth.Application.Abstractions.Persistance;
using Auth.Application.Abstractions.Security;
using Auth.Application.Features.Authentication.Login;
using Auth.Application.Features.Authentication.Responses;
using Auth.Domain.Entities;
using BuildingBlocks.Application.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;
using SharedKernel.ValueObjects;
using Xunit;

namespace Auth.UnitTests.Authentication.Login;

public class LoginCommandHandlerTests
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly IRefreshTokenRepository _refreshTokenRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IPasswordHasher _passwordHasherMock;
    private readonly IJwtProvider _jwtProviderMock;
    private readonly ILogger<LoginCommandHandler> _loggerMock;
    private readonly IRefreshTokenHasher _refreshTokenHasherMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _refreshTokenRepositoryMock = Substitute.For<IRefreshTokenRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _passwordHasherMock = Substitute.For<IPasswordHasher>();
        _jwtProviderMock = Substitute.For<IJwtProvider>();
        _loggerMock = Substitute.For<ILogger<LoginCommandHandler>>();
        _refreshTokenHasherMock = Substitute.For<IRefreshTokenHasher>();

        _handler = new LoginCommandHandler(
            _userRepositoryMock,
            _refreshTokenRepositoryMock,
            _unitOfWorkMock,
            _passwordHasherMock,
            _jwtProviderMock,
            _loggerMock,
            _refreshTokenHasherMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "password123");
        var email = Email.Create(command.Email).Value;
        _userRepositoryMock.GetUserByEmail(email!).Returns((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Invalid credentials");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPasswordIsIncorrect()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "wrongpassword");
        var email = Email.Create(command.Email).Value;
        var user = User.Create("Test", command.Email, "hashed_password").Value;
        
        _userRepositoryMock.GetUserByEmail(email!).Returns(user);
        _passwordHasherMock.VerifyPassword(command.Password, user!.PasswordHash).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Invalid credentials");
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "correctpassword");
        var email = Email.Create(command.Email).Value;
        var user = User.Create("Test", command.Email, "hashed_password").Value;
        var tokenResponse = new TokenResponse("access_token", "refresh_token", DateTime.UtcNow.AddMinutes(60));

        _userRepositoryMock.GetUserByEmail(email!).Returns(user);
        _passwordHasherMock.VerifyPassword(command.Password, user!.PasswordHash).Returns(true);
        _jwtProviderMock.Generate(user).Returns(tokenResponse);
        _refreshTokenHasherMock.Hash(tokenResponse.RefreshToken).Returns("refresh_token_hash");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(tokenResponse);
        await _refreshTokenRepositoryMock.Received(1).RevokeAllUserTokens(user.Id, Arg.Any<CancellationToken>());
        await _refreshTokenRepositoryMock.Received(1).CreateRefreshToken(Arg.Any<Auth.Domain.Entities.RefreshToken>(), Arg.Any<CancellationToken>());
        await _unitOfWorkMock.Received(1).CommitAsync();
    }

    [Fact]
    public async Task Handle_ShouldRehashPassword_WhenNeedsRehashIsTrue()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "correctpassword");
        var email = Email.Create(command.Email).Value;
        var user = User.Create("Test", command.Email, "old_hash").Value;
        var tokenResponse = new TokenResponse("access_token", "refresh_token", DateTime.UtcNow.AddMinutes(60));

        _userRepositoryMock.GetUserByEmail(email!).Returns(user);
        _passwordHasherMock.VerifyPassword(command.Password, user!.PasswordHash).Returns(true);
        _passwordHasherMock.NeedsRehash(user.PasswordHash).Returns(true);
        _passwordHasherMock.HashPassword(command.Password).Returns("new_hash");
        
        _jwtProviderMock.Generate(user).Returns(tokenResponse);
        _refreshTokenHasherMock.Hash(tokenResponse.RefreshToken).Returns("refresh_token_hash");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.PasswordHash.Should().Be("new_hash");
        await _userRepositoryMock.Received(1).UpdateUser(user);
    }
}
