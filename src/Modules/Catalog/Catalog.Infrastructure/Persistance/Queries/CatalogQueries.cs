using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Abstractions.Queries;
using Catalog.Application.Features.ListItem;
using Catalog.Infrastructure.Persistance.Sql;
using Dapper;

namespace Catalog.Infrastructure.Persistance.Queries;

public sealed class CatalogQueries(IDbConnectionFactory connectionFactory) : ICatalogQueries
{
    public async Task<CatalogListItem?> GetCategoryDetails(Guid id, Guid userId)
    {
        using var connection = connectionFactory.CreateConnection();
        var category = await connection.QueryFirstOrDefaultAsync<CatalogListItem>(
            CatalogSql.GetCategoryDetails,
            new { Id = id, UserId = userId }
        );
        return category;
    }

    public async Task<IReadOnlyList<CatalogListItem>> GetCategoriesByUserId(Guid userId)
    {
        using var connection = connectionFactory.CreateConnection();
        return (IReadOnlyList<CatalogListItem>)await connection.QueryAsync<CatalogListItem>(
            CatalogSql.GetCategoriesByUserId,
            new { UserId = userId }
        );
    }
}