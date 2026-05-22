using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Persistance;
using Finance.Application.Features.Commands.RecurringTransaction.DeleteRecurringTransaction;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;
using Xunit;

namespace Finance.UnitTests.Commands.RecurringTransaction.DeleteRecurringTransaction;

public sealed class DeleteRecurringTransactionCommandHandlerTests
{
    private readonly IRecurringTransactionRepository _recurringTransactionRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ILogger<DeleteRecurringTransactionCommandHandler> _loggerMock;
    private readonly DeleteRecurringTransactionCommandHandler _handler;

    public DeleteRecurringTransactionCommandHandlerTests()
    {
        _recurringTransactionRepositoryMock = Substitute.For<IRecurringTransactionRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<DeleteRecurringTransactionCommandHandler>>();

        _handler = new DeleteRecurringTransactionCommandHandler(
            _recurringTransactionRepositoryMock,
            _unitOfWorkMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenRecurringTransactionIsDeleted()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new DeleteRecurringTransactionCommand(id);
        _recurringTransactionRepositoryMock.DeleteRecurringTransaction(id).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _recurringTransactionRepositoryMock.Received(1).DeleteRecurringTransaction(id);
        await _unitOfWorkMock.Received(1).CommitAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRecurringTransactionNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new DeleteRecurringTransactionCommand(id);
        _recurringTransactionRepositoryMock.DeleteRecurringTransaction(id).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Recurring Transaction not found.");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }
}

