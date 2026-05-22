using Finance.Domain.Enums;

namespace Finance.Application.Features.ListItem;

public record AccountListItem(
    Guid Id,
    string Name,
    AccountType Type,
    decimal CurrentBalance,
    bool IsActive,
    DateTime CreatedAt
);