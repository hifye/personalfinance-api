using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Persistance;
using Finance.Application.Features.Commands.RecurringTransaction.PatchRecurringTransaction;
using Finance.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;

namespace Finance.UnitTests.Commands.RecurringTransaction.PatchRecurringTransaction;

public sealed class PatchRecurringTransactionCommandHandlerTests
{
    private readonly ICurrentUser _currentUserMock;
    private readonly PatchRecurringTransactionCommandHandler _handler;
    private readonly ILogger<PatchRecurringTransactionCommandHandler> _loggerMock;
    private readonly IRecurringTransactionRepository _recurringTransactionRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public PatchRecurringTransactionCommandHandlerTests()
    {
        _currentUserMock = Substitute.For<ICurrentUser>();
        _recurringTransactionRepositoryMock = Substitute.For<IRecurringTransactionRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<PatchRecurringTransactionCommandHandler>>();

        _handler = new PatchRecurringTransactionCommandHandler(
            _recurringTransactionRepositoryMock,
            _unitOfWorkMock,
            _currentUserMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenRecurringTransactionIsPatched()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var recurringTransaction = Finance.Domain.Entities.RecurringTransaction.Create(userId, Guid.NewGuid(),
            Guid.NewGuid(), 100m, TransactionType.Expense, "Old", RecurringFrequency.Monthly).Value;
        var command = new PatchRecurringTransactionCommand(id, 200m, TransactionType.Income, "New",
            RecurringFrequency.Weekly, false);

        _currentUserMock.UserId.Returns(userId);
        _recurringTransactionRepositoryMock.GetRecurringTransactionById(id, userId).Returns(recurringTransaction);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        recurringTransaction!.Amount.Value.Should().Be(200m);
        recurringTransaction.Type.Should().Be(TransactionType.Income);
        recurringTransaction.Description.Should().Be("New");
        recurringTransaction.Frequency.Should().Be(RecurringFrequency.Weekly);
        recurringTransaction.IsActive.Should().BeFalse();
        await _recurringTransactionRepositoryMock.Received(1).UpdateRecurringTransaction(recurringTransaction);
        await _unitOfWorkMock.Received(1).CommitAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRecurringTransactionNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var command = new PatchRecurringTransactionCommand(id, 200m, null, null, null, null);

        _currentUserMock.UserId.Returns(userId);
        _recurringTransactionRepositoryMock.GetRecurringTransactionById(id, userId)
            .Returns((Finance.Domain.Entities.RecurringTransaction?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Recurring Transaction not found.");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenRecurringTransactionBelongsToAnotherUser()
    {
        // Arrange
        var userAId = Guid.NewGuid();
        var userBId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var command = new PatchRecurringTransactionCommand(id, 200m, null, null, null, null);

        _currentUserMock.UserId.Returns(userBId);
        _recurringTransactionRepositoryMock.GetRecurringTransactionById(id, userBId)
            .Returns((Finance.Domain.Entities.RecurringTransaction?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        await _recurringTransactionRepositoryMock.Received(1).GetRecurringTransactionById(id, userBId);
        await _recurringTransactionRepositoryMock.DidNotReceive().GetRecurringTransactionById(id, userAId);
        await _recurringTransactionRepositoryMock.DidNotReceive()
            .UpdateRecurringTransaction(Arg.Any<Finance.Domain.Entities.RecurringTransaction>());
        await _unitOfWorkMock.DidNotReceive().CommitAsync();
    }
}