using Finance.Application.Abstractions.Queries;
using Finance.Application.Features.ListItem;
using Finance.Application.Features.Queries.Account.GetAccountDetails;
using Finance.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;
using Xunit;

namespace Finance.UnitTests.Queries.Account;

public sealed class GetAccountDetailsQueryHandlerTests
{
    private readonly IAccountQueries _accountQueriesMock;
    private readonly ILogger<GetAccountDetailsQueryHandler> _loggerMock;
    private readonly GetAccountDetailsQueryHandler _handler;

    public GetAccountDetailsQueryHandlerTests()
    {
        _accountQueriesMock = Substitute.For<IAccountQueries>();
        _loggerMock = Substitute.For<ILogger<GetAccountDetailsQueryHandler>>();

        _handler = new GetAccountDetailsQueryHandler(
            _accountQueriesMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAccountExists()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var query = new GetAccountDetailsQuery(accountId);
        var accountListItem = new AccountListItem(accountId, "Test", AccountType.Checking, 100m, true, DateTime.UtcNow);

        _accountQueriesMock.GetAccountDetails(accountId).Returns(accountListItem);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(accountListItem);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAccountDoesNotExist()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var query = new GetAccountDetailsQuery(accountId);

        _accountQueriesMock.GetAccountDetails(accountId).Returns((AccountListItem?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Account not found");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }
}

