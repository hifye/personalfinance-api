using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Queries;
using Finance.Application.Features.ListItem;
using Finance.Application.Features.Queries.RecurringTransaction.GetRecurringTransactionDetails;
using Finance.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;

namespace Finance.UnitTests.Queries.RecurringTransaction;

public sealed class GetRecurringTransactionDetailsQueryHandlerTests
{
    private readonly ICurrentUser _currentUserMock;
    private readonly GetRecurringTransactionDetailsQueryHandler _handler;
    private readonly ILogger<GetRecurringTransactionDetailsQueryHandler> _loggerMock;
    private readonly IRecurringTransactionQueries _recurringTransactionQueriesMock;

    public GetRecurringTransactionDetailsQueryHandlerTests()
    {
        _currentUserMock = Substitute.For<ICurrentUser>();
        _recurringTransactionQueriesMock = Substitute.For<IRecurringTransactionQueries>();
        _loggerMock = Substitute.For<ILogger<GetRecurringTransactionDetailsQueryHandler>>();

        _handler = new GetRecurringTransactionDetailsQueryHandler(
            _currentUserMock,
            _recurringTransactionQueriesMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenRecurringTransactionExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var query = new GetRecurringTransactionDetailsQuery(id);
        var listItem = new RecurringTransactionListItem(id, Guid.NewGuid(), Guid.NewGuid(), 100m,
            TransactionType.Expense, "Test", RecurringFrequency.Monthly, DateOnly.FromDateTime(DateTime.UtcNow), null,
            true);

        _currentUserMock.UserId.Returns(userId);
        _recurringTransactionQueriesMock.GetRecurringTransactionDetails(id, userId).Returns(listItem);

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
        var userId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var query = new GetRecurringTransactionDetailsQuery(id);

        _currentUserMock.UserId.Returns(userId);
        _recurringTransactionQueriesMock.GetRecurringTransactionDetails(id, userId)
            .Returns((RecurringTransactionListItem?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Recurring Transaction not found");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenRecurringTransactionBelongsToAnotherUser()
    {
        // Arrange
        var userAId = Guid.NewGuid();
        var userBId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var query = new GetRecurringTransactionDetailsQuery(id);

        _currentUserMock.UserId.Returns(userBId);
        _recurringTransactionQueriesMock.GetRecurringTransactionDetails(id, userBId)
            .Returns((RecurringTransactionListItem?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        await _recurringTransactionQueriesMock.Received(1).GetRecurringTransactionDetails(id, userBId);
        await _recurringTransactionQueriesMock.DidNotReceive().GetRecurringTransactionDetails(id, userAId);
    }
}