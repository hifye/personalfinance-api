using Finance.Application.Abstractions.Queries;
using Finance.Application.Features.ListItem;
using Finance.Application.Features.Queries.RecurringTransaction.GetRecurringTransactionDetails;
using Finance.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;
using Xunit;

namespace Finance.UnitTests.Queries.RecurringTransaction;

public sealed class GetRecurringTransactionDetailsQueryHandlerTests
{
    private readonly IRecurringTransactionQueries _recurringTransactionQueriesMock;
    private readonly ILogger<GetRecurringTransactionDetailsQueryHandler> _loggerMock;
    private readonly GetRecurringTransactionDetailsQueryHandler _handler;

    public GetRecurringTransactionDetailsQueryHandlerTests()
    {
        _recurringTransactionQueriesMock = Substitute.For<IRecurringTransactionQueries>();
        _loggerMock = Substitute.For<ILogger<GetRecurringTransactionDetailsQueryHandler>>();

        _handler = new GetRecurringTransactionDetailsQueryHandler(
            _recurringTransactionQueriesMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenRecurringTransactionExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var query = new GetRecurringTransactionDetailsQuery(id);
        var listItem = new RecurringTransactionListItem(id, Guid.NewGuid(), Guid.NewGuid(), 100m, TransactionType.Expense, "Test", RecurringFrequency.Monthly, DateOnly.FromDateTime(DateTime.UtcNow), null, true);

        _recurringTransactionQueriesMock.GetRecurringTransactionDetails(id).Returns(listItem);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(listItem);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRecurringTransactionDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        var query = new GetRecurringTransactionDetailsQuery(id);

        _recurringTransactionQueriesMock.GetRecurringTransactionDetails(id).Returns((RecurringTransactionListItem?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Recurring Transaction not found");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }
}

