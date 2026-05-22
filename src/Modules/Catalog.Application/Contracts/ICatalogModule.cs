namespace Catalog.Application.Contracts;

public interface ICatalogModule
{
    Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken ct = default);
}