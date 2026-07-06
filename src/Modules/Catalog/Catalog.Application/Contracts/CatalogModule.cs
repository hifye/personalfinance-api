using BuildingBlocks.Contracts;
using Catalog.Application.Abstractions.Persistance;

namespace Catalog.Application.Contracts;

internal sealed class CatalogModule(ICatalogRepository categoryRepository) : ICatalogModule
{
    public async Task<bool> CategoryExistsAsync(Guid categoryId, Guid userId, CancellationToken ct)
    {
        var category = await categoryRepository.GetCategoryById(categoryId, userId);
        return category is not null;
    }
}