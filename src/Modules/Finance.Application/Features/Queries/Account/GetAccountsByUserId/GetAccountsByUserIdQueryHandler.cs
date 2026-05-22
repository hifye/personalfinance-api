using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Queries;
using Finance.Application.Features.ListItem;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Finance.Application.Features.Queries.Account.GetAccountsByUserId;

public class GetAccountsByUserIdQueryHandler(IAccountQueries accountQueries, ICurrentUser currentUser, ILogger<GetAccountsByUserIdQueryHandler> logger)
    : IRequestHandler<GetAccountsByUserIdQuery, Result<IReadOnlyList<AccountListItem>>>
{
    public async Task<Result<IReadOnlyList<AccountListItem>>> Handle(GetAccountsByUserIdQuery query, CancellationToken cancellationToken)
    {
        var account = await accountQueries.GetAccountsByUserId(currentUser.UserId);
        
        if (account.Any()) return Result<IReadOnlyList<AccountListItem>>.Success(account);
        logger.LogWarning("No accounts found for user with ID {UserId}", currentUser.UserId);
        return Result<IReadOnlyList<AccountListItem>>.Failure("No accounts found for the user", ErrorType.NotFound);
    }
}