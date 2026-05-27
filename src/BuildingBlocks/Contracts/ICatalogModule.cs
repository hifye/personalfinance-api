namespace BuildingBlocks.Contracts;

public interface ICatalogModule
{
    Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken ct = default);
}