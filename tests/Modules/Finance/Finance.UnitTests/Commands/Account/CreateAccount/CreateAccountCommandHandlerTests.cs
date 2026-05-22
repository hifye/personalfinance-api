using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Persistance;
using Finance.Application.Features.Commands.Account.CreateAccount;
using Finance.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using SharedKernel.Common;
using Xunit;

namespace Finance.UnitTests.Commands.Account.CreateAccount;

public sealed class CreateAccountCommandHandlerTests
{
    private readonly IAccountRepository _accountRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ICurrentUser _currentUserMock;
    private readonly CreateAccountCommandHandler _handler;

    public CreateAccountCommandHandlerTests()
    {
        _accountRepositoryMock = Substitute.For<IAccountRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _currentUserMock = Substitute.For<ICurrentUser>();

        _handler = new CreateAccountCommandHandler(
            _accountRepositoryMock,
            _unitOfWorkMock,
            _currentUserMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAccountIsCreated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateAccountCommand("Test Account", AccountType.Checking, 1000m);
        _currentUserMock.UserId.Returns(userId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        await _accountRepositoryMock.Received(1).CreateAccount(Arg.Is<Domain.Entities.Account>(a => 
            a.Name == command.Name && 
            a.Type == command.Type && 
            a.UserId == userId &&
            a.InitialBalance.Value == command.InitialBalance));
        await _unitOfWorkMock.Received(1).CommitAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenDomainValidationFails()
    {
        // Arrange
        var command = new CreateAccountCommand("", AccountType.Checking, 1000m); // Name is mandatory
        _currentUserMock.UserId.Returns(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("The field name is mandatory.");
        await _accountRepositoryMock.DidNotReceive().CreateAccount(Arg.Any<Finance.Domain.Entities.Account>());
        await _unitOfWorkMock.DidNotReceive().CommitAsync();
    }
}

