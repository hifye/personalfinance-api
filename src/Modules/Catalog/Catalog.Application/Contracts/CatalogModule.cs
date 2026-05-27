using BuildingBlocks.Contracts;
using Catalog.Application.Abstractions.Persistance;

namespace Catalog.Application.Contracts;

internal sealed class CatalogModule(ICatalogRepository categoryRepository) : ICatalogModule
{
    public async Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken ct = default)
    {
        var category = await categoryRepository.GetCategoryById(categoryId);
        return category is not null;
    }
}