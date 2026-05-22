using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Queries;
using Finance.Application.Features.ListItem;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Finance.Application.Features.Queries.Transaction.GetTransactionsByUserId;

public sealed class GetTransactionsByUserIdQueryHandler(ITransactionQueries transactionQueries, ICurrentUser currentUser, ILogger<GetTransactionsByUserIdQueryHandler> logger)
    : IRequestHandler<GetTransactionsByUserIdQuery, Result<IReadOnlyList<TransactionListItem>>>
{
    public async Task<Result<IReadOnlyList<TransactionListItem>>> Handle(GetTransactionsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await transactionQueries.GetTransactionsByUserId(currentUser.UserId);

        if (transaction.Any()) return Result<IReadOnlyList<TransactionListItem>>.Success(transaction);
        logger.LogWarning("No transactions found for user with ID {UserId}", currentUser.UserId);
        return Result<IReadOnlyList<TransactionListItem>>.Failure("No transactions found for the user", ErrorType.NotFound);

    }
}
