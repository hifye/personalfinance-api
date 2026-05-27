using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Queries;
using Finance.Application.Features.ListItem;
using Finance.Application.Features.Queries.Transaction.GetTransactionSummary;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;

namespace Finance.UnitTests.Queries.Transaction;

public sealed class GetTransactionSummaryQueryHandlerTests
{
    private readonly ITransactionQueries _transactionQueriesMock;
    private readonly ICurrentUser _currentUserMock;
    private readonly ILogger<GetTransactionSummaryQueryHandler> _loggerMock;
    private readonly GetTransactionSummaryQueryHandler _handler;

    public GetTransactionSummaryQueryHandlerTests()
    {
        _transactionQueriesMock = Substitute.For<ITransactionQueries>();
        _currentUserMock = Substitute.For<ICurrentUser>();
        _loggerMock = Substitute.For<ILogger<GetTransactionSummaryQueryHandler>>();

        _handler = new GetTransactionSummaryQueryHandler(
            _transactionQueriesMock,
            _currentUserMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenDatesAreValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.AddDays(-7);
        var endDate = DateTime.UtcNow;
        var query = new GetTransactionSummaryQuery(startDate, endDate);
        var summary = new TransactionSummary(1000m, 500m, 500m);

        _currentUserMock.UserId.Returns(userId);
        _transactionQueriesMock.GetTransactionSummary(userId, startDate, endDate).Returns(summary);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(summary);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenStartDateIsAfterEndDate()
    {
        // Arrange
        var startDate = DateTime.UtcNow;
        var endDate = DateTime.UtcNow.AddDays(-7);
        var query = new GetTransactionSummaryQuery(startDate, endDate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Start date cannot be after end date");
        result.ErrorType.Should().Be(ErrorType.Validation);
    }
}

