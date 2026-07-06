using BuildingBlocks.Application.Abstractions;
using Dapper;
using Finance.Application.Abstractions.Queries;
using Finance.Application.Features.ListItem;
using Infrastructure.Data.Sql;

namespace Finance.Infrastructure.Persistance.Queries;

public sealed class AccountQueries(IDbConnectionFactory connectionFactory) : IAccountQueries
{
    public async Task<AccountListItem> GetAccountDetails(Guid id)
    {
        using var connection = connectionFactory.CreateConnection();
        var account = await connection.QueryFirstOrDefaultAsync<AccountListItem>(AccountSql.GetAccountDetails,
            new { Id = id });
        return account ?? throw new InvalidOperationException($"Account with ID {id} not found");
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
