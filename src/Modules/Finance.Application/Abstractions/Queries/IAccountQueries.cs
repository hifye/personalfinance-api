using Finance.Application.Features.ListItem;

namespace Finance.Application.Abstractions.Queries;

public interface IAccountQueries
{
    Task<AccountListItem> GetAccountDetails(Guid id);
    Task<IReadOnlyList<AccountListItem>> GetAccountsByUserId(Guid userId);
}