using Catalog.Application.Abstractions.Queries;
using Catalog.Application.Features.ListItem;
using Catalog.Application.Features.Queries.GetCategoryDetails;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;
using Xunit;

namespace Catalog.UnitTests.Queries.GetCategoryDetails;

public class GetCategoryDetailsQueryHandlerTests
{
    private readonly ICatalogQueries _catalogQueriesMock;
    private readonly ILogger<GetCategoryDetailsQueryHandler> _loggerMock;
    private readonly GetCategoryDetailsQueryHandler _handler;

    public GetCategoryDetailsQueryHandlerTests()
    {
        _catalogQueriesMock = Substitute.For<ICatalogQueries>();
        _loggerMock = Substitute.For<ILogger<GetCategoryDetailsQueryHandler>>();

        _handler = new GetCategoryDetailsQueryHandler(
            _catalogQueriesMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCategoryExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var query = new GetCategoryDetailsQuery(categoryId);
        var catalogListItem = new CatalogListItem(
            categoryId,
            "Test Category",
            "Expense",
            true,
            DateTime.UtcNow
        );

        _catalogQueriesMock.GetCategoryDetails(categoryId).Returns(catalogListItem);

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
        var categoryId = Guid.NewGuid();
        var query = new GetCategoryDetailsQuery(categoryId);

        _catalogQueriesMock.GetCategoryDetails(categoryId).Returns((CatalogListItem?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Category not found");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }
}
