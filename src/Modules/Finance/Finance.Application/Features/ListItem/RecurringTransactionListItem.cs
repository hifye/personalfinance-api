using Finance.Domain.Enums;

namespace Finance.Application.Features.ListItem;

public record RecurringTransactionListItem(
    Guid Id,
    Guid AccountId,
    Guid CategoryId,
    decimal Amount,
    TransactionType Type,
    string Description,
    RecurringFrequency Frequency,
    DateOnly NextOccurrence,
    DateOnly? EndDate,
    bool IsActive
);