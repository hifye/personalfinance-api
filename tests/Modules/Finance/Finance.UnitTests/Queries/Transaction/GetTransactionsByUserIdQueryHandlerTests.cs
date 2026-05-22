using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Queries;
using Finance.Application.Features.ListItem;
using Finance.Application.Features.Queries.Transaction.GetTransactionsByUserId;
using Finance.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;
using Xunit;

namespace Finance.UnitTests.Queries.Transaction;

public class GetTransactionsByUserIdQueryHandlerTests
{
    private readonly ITransactionQueries _transactionQueriesMock;
    private readonly ICurrentUser _currentUserMock;
    private readonly ILogger<GetTransactionsByUserIdQueryHandler> _loggerMock;
    private readonly GetTransactionsByUserIdQueryHandler _handler;

    public GetTransactionsByUserIdQueryHandlerTests()
    {
        _transactionQueriesMock = Substitute.For<ITransactionQueries>();
        _currentUserMock = Substitute.For<ICurrentUser>();
        _loggerMock = Substitute.For<ILogger<GetTransactionsByUserIdQueryHandler>>();

        _handler = new GetTransactionsByUserIdQueryHandler(
            _transactionQueriesMock,
            _currentUserMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTransactionsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var transactions = new List<TransactionListItem>
        {
            new TransactionListItem(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, 100m, TransactionType.Expense, "T1", DateTime.UtcNow, DateTime.UtcNow),
            new TransactionListItem(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, 200m, TransactionType.Income, "T2", DateTime.UtcNow, DateTime.UtcNow)
        };

        _currentUserMock.UserId.Returns(userId);
        _transactionQueriesMock.GetTransactionsByUserId(userId).Returns(transactions);

        // Act
        var result = await _handler.Handle(new GetTransactionsByUserIdQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNoTransactionsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.UserId.Returns(userId);
        _transactionQueriesMock.GetTransactionsByUserId(userId).Returns(new List<TransactionListItem>());

        // Act
        var result = await _handler.Handle(new GetTransactionsByUserIdQuery(), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("No transactions found for the user");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }
}
