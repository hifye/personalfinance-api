using Finance.Application.Abstractions.Queries;
using Finance.Application.Features.ListItem;
using Finance.Application.Features.Queries.Transaction.GetTransactionDetails;
using Finance.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;
using Xunit;

namespace Finance.UnitTests.Queries.Transaction;

public class GetTransactionDetailsQueryHandlerTests
{
    private readonly ITransactionQueries _transactionQueriesMock;
    private readonly ILogger<GetTransactionDetailsQueryHandler> _loggerMock;
    private readonly GetTransactionDetailsQueryHandler _handler;

    public GetTransactionDetailsQueryHandlerTests()
    {
        _transactionQueriesMock = Substitute.For<ITransactionQueries>();
        _loggerMock = Substitute.For<ILogger<GetTransactionDetailsQueryHandler>>();

        _handler = new GetTransactionDetailsQueryHandler(
            _transactionQueriesMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTransactionExists()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var query = new GetTransactionDetailsQuery(transactionId);
        var transactionListItem = new TransactionListItem(transactionId, Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, 100m, TransactionType.Expense, "Test", DateTime.UtcNow, DateTime.UtcNow);

        _transactionQueriesMock.GetTransactionDetails(transactionId).Returns(transactionListItem);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(transactionListItem);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTransactionDoesNotExist()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var query = new GetTransactionDetailsQuery(transactionId);

        _transactionQueriesMock.GetTransactionDetails(transactionId).Returns((TransactionListItem?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Transaction not found");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }
}
