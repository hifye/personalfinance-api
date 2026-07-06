using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Persistance;
using Finance.Application.Features.Commands.Transaction.DeleteTransaction;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;

namespace Finance.UnitTests.Commands.Transaction.DeleteTransaction;

public sealed class DeleteTransactionCommandHandlerTests
{
    private readonly ICurrentUser _currentUserMock;
    private readonly DeleteTransactionCommandHandler _handler;
    private readonly ILogger<DeleteTransactionCommandHandler> _loggerMock;
    private readonly ITransactionRepository _transactionRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public DeleteTransactionCommandHandlerTests()
    {
        _currentUserMock = Substitute.For<ICurrentUser>();
        _transactionRepositoryMock = Substitute.For<ITransactionRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<DeleteTransactionCommandHandler>>();

        _handler = new DeleteTransactionCommandHandler(
            _currentUserMock,
            _transactionRepositoryMock,
            _unitOfWorkMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTransactionIsDeleted()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var command = new DeleteTransactionCommand(transactionId);

        _currentUserMock.UserId.Returns(userId);
        _transactionRepositoryMock.DeleteTransaction(transactionId, userId).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _transactionRepositoryMock.Received(1).DeleteTransaction(transactionId, userId);
        await _unitOfWorkMock.Received(1).CommitAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTransactionNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var command = new DeleteTransactionCommand(transactionId);

        _currentUserMock.UserId.Returns(userId);
        _transactionRepositoryMock.DeleteTransaction(transactionId, userId).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Transaction not found.");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenTransactionBelongsToAnotherUser()
    {
        // Arrange
        var userAId = Guid.NewGuid();
        var userBId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var command = new DeleteTransactionCommand(transactionId);

        _currentUserMock.UserId.Returns(userBId);
        _transactionRepositoryMock.DeleteTransaction(transactionId, userBId).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        await _transactionRepositoryMock.Received(1).DeleteTransaction(transactionId, userBId);
        await _transactionRepositoryMock.DidNotReceive().DeleteTransaction(transactionId, userAId);
        await _unitOfWorkMock.DidNotReceive().CommitAsync();
    }
}