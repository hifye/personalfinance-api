using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Persistance;
using Finance.Application.Features.Commands.Transaction.PatchTransaction;
using Finance.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;

namespace Finance.UnitTests.Commands.Transaction.PatchTransaction;

public sealed class PatchTransactionCommandHandlerTests
{
    private readonly ICurrentUser _currentUserMock;
    private readonly PatchTransactionCommandHandler _handler;
    private readonly ILogger<PatchTransactionCommandHandler> _loggerMock;
    private readonly ITransactionRepository _transactionRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public PatchTransactionCommandHandlerTests()
    {
        _currentUserMock = Substitute.For<ICurrentUser>();
        _transactionRepositoryMock = Substitute.For<ITransactionRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<PatchTransactionCommandHandler>>();

        _handler = new PatchTransactionCommandHandler(
            _currentUserMock,
            _transactionRepositoryMock,
            _unitOfWorkMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTransactionIsPatched()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var transaction = Finance.Domain.Entities.Transaction.Create(userId, Guid.NewGuid(), Guid.NewGuid(), null, 100m,
            TransactionType.Expense, "Old Description").Value;
        var command = new PatchTransactionCommand(transactionId, TransactionType.Income, "New Description");

        _currentUserMock.UserId.Returns(userId);
        _transactionRepositoryMock.GetTransactionById(transactionId, userId).Returns(transaction);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        transaction!.Type.Should().Be(TransactionType.Income);
        transaction.Description.Should().Be("New Description");
        await _transactionRepositoryMock.Received(1).UpdateTransaction(transaction);
        await _unitOfWorkMock.Received(1).CommitAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTransactionNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var command = new PatchTransactionCommand(transactionId, TransactionType.Income, "New Description");

        _currentUserMock.UserId.Returns(userId);
        _transactionRepositoryMock.GetTransactionById(transactionId, userId)
            .Returns((Finance.Domain.Entities.Transaction?)null);

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
        var command = new PatchTransactionCommand(transactionId, TransactionType.Income, "New Description");

        _currentUserMock.UserId.Returns(userBId);
        _transactionRepositoryMock.GetTransactionById(transactionId, userBId)
            .Returns((Finance.Domain.Entities.Transaction?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        await _transactionRepositoryMock.Received(1).GetTransactionById(transactionId, userBId);
        await _transactionRepositoryMock.DidNotReceive().GetTransactionById(transactionId, userAId);
        await _transactionRepositoryMock.DidNotReceive()
            .UpdateTransaction(Arg.Any<Finance.Domain.Entities.Transaction>());
        await _unitOfWorkMock.DidNotReceive().CommitAsync();
    }
}