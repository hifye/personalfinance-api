using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Catalog.Application.Features.Commands.CreateCategory;
using Catalog.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace PersonalFinance.IntegrationTests;

public class CatalogTests : BaseIntegrationTest
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public CatalogTests(IntegrationTestFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateCategory_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        var command = new CreateCategoryCommand("Educação", CatalogType.Expense);

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/catalog/create-category", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetCategories_ShouldReturnCategories_WhenUserHasCategories()
    {
        // Arrange
        var command = new CreateCategoryCommand("Salário", CatalogType.Income);
        await HttpClient.PostAsJsonAsync("/api/catalog/create-category", command);

        // Act
        var response = await HttpClient.GetAsync("/api/catalog/get-categories-user");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var categories = await response.Content.ReadFromJsonAsync<List<CategoryResponse>>(_jsonOptions);
        categories.Should().NotBeEmpty();
        categories.Should().Contain(c => c.Name == "Salário");
    }
    
    private record CategoryResponse(Guid Id, string Name, CatalogType Type);
}
