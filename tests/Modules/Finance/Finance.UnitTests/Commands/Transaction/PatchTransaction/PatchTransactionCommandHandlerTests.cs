using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Persistance;
using Finance.Application.Features.Commands.Transaction.PatchTransaction;
using Finance.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;
using Xunit;

namespace Finance.UnitTests.Commands.Transaction.PatchTransaction;

public sealed class PatchTransactionCommandHandlerTests
{
    private readonly ITransactionRepository _transactionRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ILogger<PatchTransactionCommandHandler> _loggerMock;
    private readonly PatchTransactionCommandHandler _handler;

    public PatchTransactionCommandHandlerTests()
    {
        _transactionRepositoryMock = Substitute.For<ITransactionRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<PatchTransactionCommandHandler>>();

        _handler = new PatchTransactionCommandHandler(
            _transactionRepositoryMock,
            _unitOfWorkMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTransactionIsPatched()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var transaction = Finance.Domain.Entities.Transaction.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, 100m, TransactionType.Expense, "Old Description").Value;
        var command = new PatchTransactionCommand(transactionId, TransactionType.Income, "New Description");

        _transactionRepositoryMock.GetTransactionById(transactionId).Returns(transaction);

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
        var transactionId = Guid.NewGuid();
        var command = new PatchTransactionCommand(transactionId, TransactionType.Income, "New Description");

        _transactionRepositoryMock.GetTransactionById(transactionId).Returns((Finance.Domain.Entities.Transaction?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Transaction not found.");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }
}

