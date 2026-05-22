using Finance.Application.Abstractions.Queries;
using Finance.Application.Features.ListItem;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Finance.Application.Features.Queries.Transaction.GetTransactionDetails;

public class GetTransactionDetailsQueryHandler(ITransactionQueries transactionQueries, ILogger<GetTransactionDetailsQueryHandler> logger)
    : IRequestHandler<GetTransactionDetailsQuery, Result<TransactionListItem>>
{
    public async Task<Result<TransactionListItem>> Handle(GetTransactionDetailsQuery query, CancellationToken cancellationToken)
    {
        var transaction = await transactionQueries.GetTransactionDetails(query.Id);

        if (transaction is not null) 
            return Result<TransactionListItem>.Success(transaction);
        
        logger.LogWarning("Transaction with ID {TransactionId} not found for query ID {QueryId}", transaction?.Id, query.Id);
        return Result<TransactionListItem>.Failure("Transaction not found", ErrorType.NotFound);

    }
}