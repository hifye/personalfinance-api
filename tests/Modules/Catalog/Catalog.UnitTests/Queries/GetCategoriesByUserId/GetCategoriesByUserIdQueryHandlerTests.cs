using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Abstractions.Queries;
using Catalog.Application.Features.ListItem;
using Catalog.Application.Features.Queries.GetCategoriesByUserId;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;
using Xunit;

namespace Catalog.UnitTests.Queries.GetCategoriesByUserId;

public class GetCategoriesByUserIdQueryHandlerTests
{
    private readonly ICatalogQueries _catalogQueriesMock;
    private readonly ICurrentUser _currentUserMock;
    private readonly ILogger<GetCategoriesByUserIdQueryHandler> _loggerMock;
    private readonly GetCategoriesByUserIdQueryHandler _handler;

    public GetCategoriesByUserIdQueryHandlerTests()
    {
        _catalogQueriesMock = Substitute.For<ICatalogQueries>();
        _currentUserMock = Substitute.For<ICurrentUser>();
        _loggerMock = Substitute.For<ILogger<GetCategoriesByUserIdQueryHandler>>();

        _handler = new GetCategoriesByUserIdQueryHandler(
            _catalogQueriesMock,
            _currentUserMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCategoriesExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.UserId.Returns(userId);
        var categories = new List<CatalogListItem>
        {
            new CatalogListItem(Guid.NewGuid(), "Cat 1", Catalog.Domain.Enums.CatalogType.Expense, true, DateTime.UtcNow),
            new CatalogListItem(Guid.NewGuid(), "Cat 2", Catalog.Domain.Enums.CatalogType.Income, true, DateTime.UtcNow)
        };

        _catalogQueriesMock.GetCategoriesByUserId(userId).Returns(categories);

        // Act
        var result = await _handler.Handle(new GetCategoriesByUserIdQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNoCategoriesExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.UserId.Returns(userId);
        _catalogQueriesMock.GetCategoriesByUserId(userId).Returns(new List<CatalogListItem>());

        // Act
        var result = await _handler.Handle(new GetCategoriesByUserIdQuery(), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("No categories found for the user");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }
}
