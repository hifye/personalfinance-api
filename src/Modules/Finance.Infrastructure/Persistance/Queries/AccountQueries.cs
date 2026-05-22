using BuildingBlocks.Application.Abstractions;
using Dapper;
using Finance.Application.Abstractions.Queries;
using Finance.Application.Features.ListItem;
using Infrastructure.Data.Sql;

namespace Finance.Infrastructure.Persistance.Queries;

public class AccountQueries(IDbConnectionFactory connectionFactory) : IAccountQueries
{
    public async Task<AccountListItem> GetAccountDetails(Guid id)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryFirstOrDefaultAsync<AccountListItem>(AccountSql.GetAccountDetails,
            new { Id = id }))!;
    }


    public async Task<IReadOnlyList<AccountListItem>> GetAccountsByUserId(Guid userId)
    {
        using var connection = connectionFactory.CreateConnection();
        return (IReadOnlyList<AccountListItem>)await connection.QueryAsync<AccountListItem>(
            AccountSql.GetAccountsByUserId,
            new { UserId = userId }
        );
    }
}