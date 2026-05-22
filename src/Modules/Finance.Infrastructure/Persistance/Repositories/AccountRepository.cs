using BuildingBlocks.Application.Abstractions;
using Dapper;
using Finance.Application.Abstractions.Persistance;
using Finance.Domain.Entities;
using Infrastructure.Data.Sql;

namespace Finance.Infrastructure.Persistance.Repositories;

public sealed class AccountRepository(IUnitOfWork unitOfWork, IDbConnectionFactory connectionFactory)
    : IAccountRepository
{
    public async Task<Account?> GetAccountById(Guid id)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Account>(
            AccountSql.GetAccountById,
            new { Id = id }
        );
    }


    public async Task<int> CreateAccount(Account account)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(
            AccountSql.CreateAccount,
            new
            {
                account.UserId,
                account.Name,
                account.Type,
                account.InitialBalance,
                account.IsActive,
                account.CreatedAt,
                account.CurrentBalance
            },
            transaction: unitOfWork.Transaction
        );
    }


    public async Task<bool> UpdateAccount(Account account)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(
            AccountSql.UpdateAccount,
            new
            {
                account.Id,
                account.Type,
                account.IsActive
            },
            transaction: unitOfWork.Transaction
        ) > 0;
    }


    public async Task<bool> DeleteAccount(Guid id)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(
            AccountSql.DeleteAccount,
            new { Id = id },
            transaction: unitOfWork.Transaction
        ) > 0;
    }
}
