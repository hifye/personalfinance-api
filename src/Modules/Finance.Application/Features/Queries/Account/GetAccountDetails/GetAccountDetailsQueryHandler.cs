using Finance.Application.Abstractions.Queries;
using Finance.Application.Features.ListItem;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Finance.Application.Features.Queries.Account.GetAccountDetails;

public class GetAccountDetailsQueryHandler(IAccountQueries accountQueries, ILogger<GetAccountDetailsQueryHandler> logger)
    : IRequestHandler<GetAccountDetailsQuery, Result<AccountListItem>>
{
    public async Task<Result<AccountListItem>> Handle(GetAccountDetailsQuery query, CancellationToken cancellationToken)
    {
        var account = await accountQueries.GetAccountDetails(query.Id);
        
        if (account is not null) return Result<AccountListItem>.Success(account);
        logger.LogWarning("Account with ID {AccountId} not found", query.Id);
        return Result<AccountListItem>.Failure("Account not found", ErrorType.NotFound);
    }
}