using Finance.Application.Features.ListItem;

namespace Finance.Application.Abstractions.Queries;

public interface ITransactionQueries
{
    Task<TransactionListItem?> GetTransactionDetails(Guid id, Guid userId);
    Task<IReadOnlyList<TransactionListItem>> GetTransactionsByUserId(Guid userId);
    Task<TransactionSummary?> GetTransactionSummary(Guid userId, DateTime startDate, DateTime endDate);
}