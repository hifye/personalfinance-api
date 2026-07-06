using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Persistance;
using Finance.Application.Features.Commands.Account.DeleteAccount;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;

namespace Finance.UnitTests.Commands.Account.DeleteAccount;

public sealed class DeleteAccountCommandHandlerTests
{
    private readonly IAccountRepository _accountRepositoryMock;
    private readonly ICurrentUser _currentUserMock;
    private readonly DeleteAccountCommandHandler _handler;
    private readonly ILogger<DeleteAccountCommandHandler> _loggerMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public DeleteAccountCommandHandlerTests()
    {
        _currentUserMock = Substitute.For<ICurrentUser>();
        _accountRepositoryMock = Substitute.For<IAccountRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<DeleteAccountCommandHandler>>();

        _handler = new DeleteAccountCommandHandler(
            _currentUserMock,
            _accountRepositoryMock,
            _unitOfWorkMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAccountIsDeleted()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var command = new DeleteAccountCommand(accountId);

        _currentUserMock.UserId.Returns(userId);
        _accountRepositoryMock.DeleteAccount(accountId, userId).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _accountRepositoryMock.Received(1).DeleteAccount(accountId, userId);
        await _unitOfWorkMock.Received(1).CommitAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAccountNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var command = new DeleteAccountCommand(accountId);

        _currentUserMock.UserId.Returns(userId);
        _accountRepositoryMock.DeleteAccount(accountId, userId).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Account not found.");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenAccountBelongsToAnotherUser()
    {
        // Arrange
        var userAId = Guid.NewGuid();
        var userBId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var command = new DeleteAccountCommand(accountId);

        _currentUserMock.UserId.Returns(userBId);
        _accountRepositoryMock.DeleteAccount(accountId, userBId).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        await _accountRepositoryMock.Received(1).DeleteAccount(accountId, userBId);
        await _accountRepositoryMock.DidNotReceive().DeleteAccount(accountId, userAId);
        await _unitOfWorkMock.DidNotReceive().CommitAsync();
    }
}