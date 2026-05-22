using BuildingBlocks.Application.Abstractions;
using Dapper;
using Finance.Application.Abstractions.Queries;
using Finance.Application.Features.ListItem;
using Finance.Domain.Enums;
using Finance.Infrastructure.Persistance.Queries.Models;
using Finance.Infrastructure.Persistance.Sql;

namespace Finance.Infrastructure.Persistance.Queries;

public sealed class RecurringTransactionQueries(IDbConnectionFactory connectionFactory)
    : IRecurringTransactionQueries
{
    public async Task<RecurringTransactionListItem> GetRecurringTransactionDetails(Guid id)
    {
        using var connection = connectionFactory.CreateConnection();
        var recurringTransaction =
            await connection.QueryFirstOrDefaultAsync<RecurringTransactionDbModel>(
                RecurringTransactionSql.GetRecurringTransactionDetails,
                new { Id = id }
            );

        if (recurringTransaction is null)
            return null;

        return new RecurringTransactionListItem(
            recurringTransaction.Id,
            recurringTransaction.AccountId,
            recurringTransaction.CategoryId,
            recurringTransaction.Amount,
            (TransactionType)recurringTransaction.Type,
            recurringTransaction.Description,
            (RecurringFrequency)recurringTransaction.Frequency,
            recurringTransaction.NextOccurrence,
            recurringTransaction.EndDate,
            recurringTransaction.IsActive
        );
    }

    public async Task<IReadOnlyList<RecurringTransactionListItem>> GetRecurringTransactionsByUserId(
        Guid userId
    )
    {
        using var connection = connectionFactory.CreateConnection();
        return (IReadOnlyList<RecurringTransactionListItem>)
            await connection.QueryAsync<RecurringTransactionListItem>(
                RecurringTransactionSql.GetRecurringTransactionsByUserId,
                new { UserId = userId }
            );
    }
}

