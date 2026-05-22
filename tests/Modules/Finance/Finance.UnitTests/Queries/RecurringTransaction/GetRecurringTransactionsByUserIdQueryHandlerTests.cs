using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Queries;
using Finance.Application.Features.ListItem;
using Finance.Application.Features.Queries.RecurringTransaction.GetRecurringTransactionsByUserId;
using Finance.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;
using Xunit;

namespace Finance.UnitTests.Queries.RecurringTransaction;

public class GetRecurringTransactionsByUserIdQueryHandlerTests
{
    private readonly IRecurringTransactionQueries _recurringTransactionQueriesMock;
    private readonly ICurrentUser _currentUserMock;
    private readonly ILogger<GetRecurringTransactionsByUserIdQueryHandler> _loggerMock;
    private readonly GetRecurringTransactionsByUserIdQueryHandler _handler;

    public GetRecurringTransactionsByUserIdQueryHandlerTests()
    {
        _recurringTransactionQueriesMock = Substitute.For<IRecurringTransactionQueries>();
        _currentUserMock = Substitute.For<ICurrentUser>();
        _loggerMock = Substitute.For<ILogger<GetRecurringTransactionsByUserIdQueryHandler>>();

        _handler = new GetRecurringTransactionsByUserIdQueryHandler(
            _recurringTransactionQueriesMock,
            _currentUserMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenRecurringTransactionsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var transactions = new List<RecurringTransactionListItem>
        {
            new RecurringTransactionListItem(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 100m, TransactionType.Expense, "R1", RecurringFrequency.Monthly, DateOnly.FromDateTime(DateTime.UtcNow), null, true),
            new RecurringTransactionListItem(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 200m, TransactionType.Income, "R2", RecurringFrequency.Weekly, DateOnly.FromDateTime(DateTime.UtcNow), null, true)
        };

        _currentUserMock.UserId.Returns(userId);
        _recurringTransactionQueriesMock.GetRecurringTransactionsByUserId(userId).Returns(transactions);

        // Act
        var result = await _handler.Handle(new GetRecurringTransactionsByUserIdQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNoRecurringTransactionsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.UserId.Returns(userId);
        _recurringTransactionQueriesMock.GetRecurringTransactionsByUserId(userId).Returns(new List<RecurringTransactionListItem>());

        // Act
        var result = await _handler.Handle(new GetRecurringTransactionsByUserIdQuery(), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("No recurring transactions found for the user");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }
}
