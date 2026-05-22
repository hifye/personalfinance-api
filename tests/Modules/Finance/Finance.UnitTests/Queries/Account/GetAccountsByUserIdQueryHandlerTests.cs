using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Queries;
using Finance.Application.Features.ListItem;
using Finance.Application.Features.Queries.Account.GetAccountsByUserId;
using Finance.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;
using Xunit;

namespace Finance.UnitTests.Queries.Account;

public sealed class GetAccountsByUserIdQueryHandlerTests
{
    private readonly IAccountQueries _accountQueriesMock;
    private readonly ICurrentUser _currentUserMock;
    private readonly ILogger<GetAccountsByUserIdQueryHandler> _loggerMock;
    private readonly GetAccountsByUserIdQueryHandler _handler;

    public GetAccountsByUserIdQueryHandlerTests()
    {
        _accountQueriesMock = Substitute.For<IAccountQueries>();
        _currentUserMock = Substitute.For<ICurrentUser>();
        _loggerMock = Substitute.For<ILogger<GetAccountsByUserIdQueryHandler>>();

        _handler = new GetAccountsByUserIdQueryHandler(
            _accountQueriesMock,
            _currentUserMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAccountsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var accounts = new List<AccountListItem>
        {
            new AccountListItem(Guid.NewGuid(), "Acc 1", AccountType.Checking, 100m, true, DateTime.UtcNow),
            new AccountListItem(Guid.NewGuid(), "Acc 2", AccountType.Savings, 200m, true, DateTime.UtcNow)
        };

        _currentUserMock.UserId.Returns(userId);
        _accountQueriesMock.GetAccountsByUserId(userId).Returns(accounts);

        // Act
        var result = await _handler.Handle(new GetAccountsByUserIdQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNoAccountsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.UserId.Returns(userId);
        _accountQueriesMock.GetAccountsByUserId(userId).Returns(new List<AccountListItem>());

        // Act
        var result = await _handler.Handle(new GetAccountsByUserIdQuery(), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("No accounts found for the user");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }
}

