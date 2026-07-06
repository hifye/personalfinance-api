using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Queries;
using Finance.Application.Features.ListItem;
using Finance.Application.Features.Queries.Account.GetAccountDetails;
using Finance.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;

namespace Finance.UnitTests.Queries.Account;

public sealed class GetAccountDetailsQueryHandlerTests
{
    private readonly IAccountQueries _accountQueriesMock;
    private readonly ICurrentUser _currentUserMock;
    private readonly GetAccountDetailsQueryHandler _handler;
    private readonly ILogger<GetAccountDetailsQueryHandler> _loggerMock;

    public GetAccountDetailsQueryHandlerTests()
    {
        _currentUserMock = Substitute.For<ICurrentUser>();
        _accountQueriesMock = Substitute.For<IAccountQueries>();
        _loggerMock = Substitute.For<ILogger<GetAccountDetailsQueryHandler>>();

        _handler = new GetAccountDetailsQueryHandler(
            _currentUserMock,
            _accountQueriesMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAccountExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var query = new GetAccountDetailsQuery(accountId);
        var accountListItem = new AccountListItem(accountId, "Test", AccountType.Checking, 100m, true, DateTime.UtcNow);

        _currentUserMock.UserId.Returns(userId);
        _accountQueriesMock.GetAccountDetails(accountId, userId).Returns(accountListItem);

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
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var query = new GetAccountDetailsQuery(accountId);

        _currentUserMock.UserId.Returns(userId);
        _accountQueriesMock.GetAccountDetails(accountId, userId).Returns((AccountListItem?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Account not found");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenAccountBelongsToAnotherUser()
    {
        // Arrange
        var userAId = Guid.NewGuid();
        var userBId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var query = new GetAccountDetailsQuery(accountId);

        _currentUserMock.UserId.Returns(userBId);
        _accountQueriesMock.GetAccountDetails(accountId, userBId).Returns((AccountListItem?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        await _accountQueriesMock.Received(1).GetAccountDetails(accountId, userBId);
        await _accountQueriesMock.DidNotReceive().GetAccountDetails(accountId, userAId);
    }
}