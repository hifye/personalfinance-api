using Finance.Application.Features.ListItem;
using MediatR;
using SharedKernel.Common;

namespace Finance.Application.Features.Queries.Account.GetAccountDetails;

public record GetAccountDetailsQuery(Guid Id) : IRequest<Result<AccountListItem>>;