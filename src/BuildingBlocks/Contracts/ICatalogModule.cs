namespace BuildingBlocks.Contracts;

public interface ICatalogModule
{
    Task<bool> CategoryExistsAsync(Guid categoryId, Guid userId, CancellationToken ct);
}