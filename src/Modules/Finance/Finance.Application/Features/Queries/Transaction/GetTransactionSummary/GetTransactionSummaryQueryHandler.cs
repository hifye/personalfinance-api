using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Queries;
using Finance.Application.Features.ListItem;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Finance.Application.Features.Queries.Transaction.GetTransactionSummary;

public sealed class GetTransactionSummaryQueryHandler(
    ITransactionQueries transactionQueries,
    ICurrentUser currentUser,
    ILogger<GetTransactionSummaryQueryHandler> logger
) : IRequestHandler<GetTransactionSummaryQuery, Result<TransactionSummary>>
{
    public async Task<Result<TransactionSummary>> Handle(GetTransactionSummaryQuery query, CancellationToken cancellationToken)
    {
        if (query.StartDate > query.EndDate)
        {
            logger.LogWarning("Start date cannot be after end date");
            return Result<TransactionSummary>.Failure("Start date cannot be after end date", ErrorType.Validation);
        }

        var summary = await transactionQueries.GetTransactionSummary(
            currentUser.UserId,
            query.StartDate,
            query.EndDate
        );
        
        if (summary is null)
        {
            logger.LogInformation("No transaction summary found for user {UserId}", currentUser.UserId);
            return Result<TransactionSummary>.Failure("No transactions found for the specified period", ErrorType.NotFound);
        }

        return Result<TransactionSummary>.Success(summary);
    }
}
