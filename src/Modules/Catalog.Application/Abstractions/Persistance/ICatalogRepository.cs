namespace Catalog.Application.Abstractions.Persistance;

public interface ICatalogRepository
{
    Task<Domain.Entities.Catalog?> GetCategoryById(Guid id);
    Task<int> CreateCategory(Domain.Entities.Catalog category);
    Task<bool> PatchCategory(Domain.Entities.Catalog category);
    Task<bool> DeleteCategory(Guid id);
}