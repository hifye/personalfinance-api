using BuildingBlocks.Application.Abstractions;
using Dapper;
using Finance.Application.Abstractions.Persistance;
using Finance.Domain.Entities;
using Finance.Infrastructure.Persistance.Sql;

namespace Finance.Infrastructure.Persistance.Repositories;

public class RecurringTransactionRepository(IUnitOfWork unitOfWork, IDbConnectionFactory connectionFactory)
    : IRecurringTransactionRepository
{
    public async Task<RecurringTransaction?> GetRecurringTransactionById(Guid id)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<RecurringTransaction>(
            RecurringTransactionSql.GetRecurringTransactionById,
            new { Id = id }
        );
    }


    public async Task CreateRecurringTransaction(RecurringTransaction recurringTransaction)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            RecurringTransactionSql.CreateRecurringTransaction,
            new
            {
                recurringTransaction.UserId,
                recurringTransaction.AccountId,
                recurringTransaction.CategoryId,
                recurringTransaction.Amount,
                recurringTransaction.Type,
                recurringTransaction.Description,
                recurringTransaction.Frequency,
                recurringTransaction.NextOccurrence,
                recurringTransaction.StartDate,
                recurringTransaction.EndDate,
                recurringTransaction.IsActive,
                recurringTransaction.CreatedAt
            },
            transaction: unitOfWork.Transaction
        );
    }


    public async Task<bool> UpdateRecurringTransaction(RecurringTransaction recurringTransaction)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(
            RecurringTransactionSql.UpdateRecurringTransaction,
            new
            {
                recurringTransaction.Id,
                recurringTransaction.Amount,
                recurringTransaction.Type,
                recurringTransaction.Description,
                recurringTransaction.Frequency,
                recurringTransaction.IsActive
            },
            transaction: unitOfWork.Transaction
        ) > 0;
    }


    public async Task<bool> DeleteRecurringTransaction(Guid id)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(
            RecurringTransactionSql.DeleteRecurringTransaction,
            new { Id = id },
            transaction: unitOfWork.Transaction
        ) > 0;
    }
}