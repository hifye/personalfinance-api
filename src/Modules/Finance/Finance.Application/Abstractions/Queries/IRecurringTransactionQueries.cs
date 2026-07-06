using Finance.Application.Features.ListItem;

namespace Finance.Application.Abstractions.Queries;

public interface IRecurringTransactionQueries
{
    Task<RecurringTransactionListItem?> GetRecurringTransactionDetails(Guid id, Guid userId);
    Task<IReadOnlyList<RecurringTransactionListItem>> GetRecurringTransactionsByUserId(Guid userId);
}