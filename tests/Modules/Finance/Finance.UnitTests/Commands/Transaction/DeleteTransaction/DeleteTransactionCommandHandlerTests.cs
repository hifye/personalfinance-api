using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Persistance;
using Finance.Application.Features.Commands.Transaction.DeleteTransaction;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;
using Xunit;

namespace Finance.UnitTests.Commands.Transaction.DeleteTransaction;

public class DeleteTransactionCommandHandlerTests
{
    private readonly ITransactionRepository _transactionRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ILogger<DeleteTransactionCommandHandler> _loggerMock;
    private readonly DeleteTransactionCommandHandler _handler;

    public DeleteTransactionCommandHandlerTests()
    {
        _transactionRepositoryMock = Substitute.For<ITransactionRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<DeleteTransactionCommandHandler>>();

        _handler = new DeleteTransactionCommandHandler(
            _transactionRepositoryMock,
            _unitOfWorkMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTransactionIsDeleted()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var command = new DeleteTransactionCommand(transactionId);
        _transactionRepositoryMock.DeleteTransaction(transactionId).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _transactionRepositoryMock.Received(1).DeleteTransaction(transactionId);
        await _unitOfWorkMock.Received(1).CommitAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTransactionNotFound()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var command = new DeleteTransactionCommand(transactionId);
        _transactionRepositoryMock.DeleteTransaction(transactionId).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Transaction not found.");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }
}
