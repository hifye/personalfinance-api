using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Persistance;
using Finance.Application.Features.Commands.RecurringTransaction.PatchRecurringTransaction;
using Finance.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;
using Xunit;

namespace Finance.UnitTests.Commands.RecurringTransaction.PatchRecurringTransaction;

public class PatchRecurringTransactionCommandHandlerTests
{
    private readonly IRecurringTransactionRepository _recurringTransactionRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ILogger<PatchRecurringTransactionCommandHandler> _loggerMock;
    private readonly PatchRecurringTransactionCommandHandler _handler;

    public PatchRecurringTransactionCommandHandlerTests()
    {
        _recurringTransactionRepositoryMock = Substitute.For<IRecurringTransactionRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<PatchRecurringTransactionCommandHandler>>();

        _handler = new PatchRecurringTransactionCommandHandler(
            _recurringTransactionRepositoryMock,
            _unitOfWorkMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenRecurringTransactionIsPatched()
    {
        // Arrange
        var id = Guid.NewGuid();
        var recurringTransaction = Finance.Domain.Entities.RecurringTransaction.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 100m, TransactionType.Expense, "Old", RecurringFrequency.Monthly).Value;
        var command = new PatchRecurringTransactionCommand(id, 200m, TransactionType.Income, "New", RecurringFrequency.Weekly, false);

        _recurringTransactionRepositoryMock.GetRecurringTransactionById(id).Returns(recurringTransaction);

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
        var id = Guid.NewGuid();
        var command = new PatchRecurringTransactionCommand(id, 200m, null, null, null, null);

        _recurringTransactionRepositoryMock.GetRecurringTransactionById(id).Returns((Finance.Domain.Entities.RecurringTransaction?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Recurring Transaction not found.");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }
}
