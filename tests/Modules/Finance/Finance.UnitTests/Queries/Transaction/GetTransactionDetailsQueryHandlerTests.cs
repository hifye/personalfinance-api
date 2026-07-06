using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Queries;
using Finance.Application.Features.ListItem;
using Finance.Application.Features.Queries.Transaction.GetTransactionDetails;
using Finance.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;

namespace Finance.UnitTests.Queries.Transaction;

public sealed class GetTransactionDetailsQueryHandlerTests
{
    private readonly ICurrentUser _currentUserMock;
    private readonly GetTransactionDetailsQueryHandler _handler;
    private readonly ILogger<GetTransactionDetailsQueryHandler> _loggerMock;
    private readonly ITransactionQueries _transactionQueriesMock;

    public GetTransactionDetailsQueryHandlerTests()
    {
        _currentUserMock = Substitute.For<ICurrentUser>();
        _transactionQueriesMock = Substitute.For<ITransactionQueries>();
        _loggerMock = Substitute.For<ILogger<GetTransactionDetailsQueryHandler>>();

        _handler = new GetTransactionDetailsQueryHandler(
            _currentUserMock,
            _transactionQueriesMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTransactionExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var query = new GetTransactionDetailsQuery(transactionId);
        var transactionListItem = new TransactionListItem(transactionId, Guid.NewGuid(), Guid.NewGuid(), Guid.Empty,
            100m, TransactionType.Expense, "Test", DateTime.UtcNow, DateTime.UtcNow);

        _currentUserMock.UserId.Returns(userId);
        _transactionQueriesMock.GetTransactionDetails(transactionId, userId).Returns(transactionListItem);

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
        var userId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var query = new GetTransactionDetailsQuery(transactionId);

        _currentUserMock.UserId.Returns(userId);
        _transactionQueriesMock.GetTransactionDetails(transactionId, userId).Returns((TransactionListItem?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Transaction not found");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenTransactionBelongsToAnotherUser()
    {
        // Arrange
        var userAId = Guid.NewGuid();
        var userBId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var query = new GetTransactionDetailsQuery(transactionId);

        _currentUserMock.UserId.Returns(userBId);
        _transactionQueriesMock.GetTransactionDetails(transactionId, userBId).Returns((TransactionListItem?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        await _transactionQueriesMock.Received(1).GetTransactionDetails(transactionId, userBId);
        await _transactionQueriesMock.DidNotReceive().GetTransactionDetails(transactionId, userAId);
    }
}