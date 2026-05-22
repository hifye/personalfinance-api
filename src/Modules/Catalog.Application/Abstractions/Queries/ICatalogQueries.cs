using Catalog.Application.Features.ListItem;

namespace Catalog.Application.Abstractions.Queries;

public interface ICatalogQueries
{
    Task<CatalogListItem?> GetCategoryDetails(Guid id);
    Task<IReadOnlyList<CatalogListItem>> GetCategoriesByUserId(Guid userId);
}