using Finance.Application.Features.ListItem;
using MediatR;
using SharedKernel.Common;

namespace Finance.Application.Features.Queries.Account.GetAccountsByUserId;

public record GetAccountsByUserIdQuery : IRequest<Result<IReadOnlyList<AccountListItem>>>;