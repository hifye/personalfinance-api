using BuildingBlocks.Application.Abstractions;
using Dapper;
using Finance.Application.Abstractions.Queries;
using Finance.Application.Features.ListItem;
using Finance.Domain.Enums;
using Finance.Infrastructure.Persistance.Sql;

namespace Finance.Infrastructure.Persistance.Queries;

public sealed class TransactionQueries(IDbConnectionFactory connectionFactory) : ITransactionQueries
{
    public async Task<TransactionListItem?> GetTransactionDetails(Guid id, Guid userId)
    {
        using var connection = connectionFactory.CreateConnection();
        var transaction = await connection.QueryFirstOrDefaultAsync<TransactionListItem>(
            TransactionSql.GetTransactionDetails,
            new { Id = id, UserId = userId }
        );
        return transaction;
    }

    public async Task<IReadOnlyList<TransactionListItem>> GetTransactionsByUserId(Guid userId)
    {
        using var connection = connectionFactory.CreateConnection();
        return (IReadOnlyList<TransactionListItem>)
            await connection.QueryAsync<TransactionListItem>(
                TransactionSql.GetTransactionsByUserId,
                new { UserId = userId }
            );
    }

    public async Task<TransactionSummary> GetTransactionSummary(
        Guid userId,
        DateTime startDate,
        DateTime endDate
    )
    {
        using var connection = connectionFactory.CreateConnection();
        var summary = await connection.QueryFirstOrDefaultAsync<TransactionSummary>(
            TransactionSql.GetTransactionSummary,
            new
            {
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate,
                IncomeType = (short)TransactionType.Income,
                ExpenseType = (short)TransactionType.Expense
            }
        );
        return summary ?? throw new InvalidOperationException($"Transaction summary for user {userId} not found");
    }
}