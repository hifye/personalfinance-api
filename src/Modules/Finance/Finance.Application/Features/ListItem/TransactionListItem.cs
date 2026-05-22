using Finance.Domain.Enums;

namespace Finance.Application.Features.ListItem;

public record TransactionListItem(
    Guid Id,
    Guid AccountId,
    Guid CategoryId,
    Guid RecurringId,
    decimal Amount,
    TransactionType Type,
    string Description,
    DateTime TransactionDate,
    DateTime UpdatedAt
);