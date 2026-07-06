using BuildingBlocks.Application.Abstractions;
using Dapper;
using Finance.Application.Abstractions.Persistance;
using Finance.Domain.Entities;
using Finance.Infrastructure.Persistance.Sql;

namespace Finance.Infrastructure.Persistance.Repositories;

public sealed class TransactionRepository(IUnitOfWork unitOfWork, IDbConnectionFactory connectionFactory)
    : ITransactionRepository
{
    public async Task<Transaction?> GetTransactionById(Guid id, Guid userId)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Transaction>(
            TransactionSql.GetTransactionById,
            new { Id = id, UserId = userId }
        );
    }


    public async Task CreateTransaction(Transaction transaction)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            TransactionSql.CreateTransaction,
            new
            {
                transaction.UserId,
                transaction.AccountId,
                transaction.CategoryId,
                transaction.RecurringId,
                transaction.Amount,
                transaction.Type,
                transaction.Description,
                transaction.CreatedAt,
                transaction.TransactionDate
            },
            transaction: unitOfWork.Transaction
        );
    }


    public async Task<bool> UpdateTransaction(Transaction transaction)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(
            TransactionSql.UpdateTransaction,
            new
            {
                transaction.Id,
                transaction.Amount,
                transaction.Type,
                transaction.Description,
                transaction.UpdatedAt
            },
            transaction: unitOfWork.Transaction
        ) > 0;
    }


    public async Task<bool> DeleteTransaction(Guid id, Guid userId)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(TransactionSql.DeleteTransaction, new { Id = id, UserId = userId },
            transaction: unitOfWork.Transaction) > 0;
    }
}