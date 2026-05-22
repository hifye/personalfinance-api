using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Abstractions.Persistance;
using Catalog.Infrastructure.Persistance.Sql;
using Dapper;

namespace Catalog.Infrastructure.Persistance.Repositories;

public sealed class CatalogRepository(IUnitOfWork unitOfWork, IDbConnectionFactory connectionFactory) : ICatalogRepository
{
    public async Task<Domain.Entities.Catalog?> GetCategoryById(Guid id)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Domain.Entities.Catalog>(
            CatalogSql.GetCategoryById,
            new { Id = id }
        );
    }


    public async Task<int> CreateCategory(Domain.Entities.Catalog category)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(
            CatalogSql.CreateCategory,
            new
            {
                category.UserId,
                category.Name,
                category.Type,
                category.IsActive,
                category.CreatedAt
            },
            transaction: unitOfWork.Transaction
        );
    }


    public async Task<bool> PatchCategory(Domain.Entities.Catalog category)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(
            CatalogSql.UpdateCategory,
            new
            {
                category.Id,
                category.Name,
                category.Type,
                category.IsActive
            },
            transaction: unitOfWork.Transaction
        ) > 0;
    }


    public async Task<bool> DeleteCategory(Guid id)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(
            CatalogSql.DeleteCategory,
            new { Id = id },
            transaction: unitOfWork.Transaction
        ) > 0;
    }
}