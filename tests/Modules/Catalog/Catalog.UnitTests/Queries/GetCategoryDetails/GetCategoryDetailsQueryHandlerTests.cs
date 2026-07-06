using Catalog.Application.Abstractions.Queries;
using Catalog.Application.Features.ListItem;
using Catalog.Application.Features.Queries.GetCategoryDetails;
using BuildingBlocks.Application.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;

namespace Catalog.UnitTests.Queries.GetCategoryDetails;

public sealed class GetCategoryDetailsQueryHandlerTests
{
    private readonly ICatalogQueries _catalogQueriesMock;
    private readonly ICurrentUser _currentUserMock;
    private readonly GetCategoryDetailsQueryHandler _handler;
    private readonly ILogger<GetCategoryDetailsQueryHandler> _loggerMock;

    public GetCategoryDetailsQueryHandlerTests()
    {
        _currentUserMock = Substitute.For<ICurrentUser>();
        _catalogQueriesMock = Substitute.For<ICatalogQueries>();
        _loggerMock = Substitute.For<ILogger<GetCategoryDetailsQueryHandler>>();

        _handler = new GetCategoryDetailsQueryHandler(
            _currentUserMock,
            _catalogQueriesMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCategoryExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var query = new GetCategoryDetailsQuery(categoryId);
        var catalogListItem = new CatalogListItem(
            categoryId,
            "Test Category",
            Catalog.Domain.Enums.CatalogType.Expense,
            true,
            DateTime.UtcNow
        );

        _currentUserMock.UserId.Returns(userId);
        _catalogQueriesMock.GetCategoryDetails(categoryId, userId).Returns(catalogListItem);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(catalogListItem);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCategoryDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var query = new GetCategoryDetailsQuery(categoryId);

        _currentUserMock.UserId.Returns(userId);
        _catalogQueriesMock.GetCategoryDetails(categoryId, userId).Returns((CatalogListItem?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Category not found");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenCategoryBelongsToAnotherUser()
    {
        // Arrange
        var userAId = Guid.NewGuid();
        var userBId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var query = new GetCategoryDetailsQuery(categoryId);

        _currentUserMock.UserId.Returns(userBId);
        _catalogQueriesMock.GetCategoryDetails(categoryId, userBId).Returns((CatalogListItem?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        await _catalogQueriesMock.Received(1).GetCategoryDetails(categoryId, userBId);
        await _catalogQueriesMock.DidNotReceive().GetCategoryDetails(categoryId, userAId);
    }
}