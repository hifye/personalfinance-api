using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Queries;
using Finance.Application.Features.ListItem;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Finance.Application.Features.Queries.RecurringTransaction.GetRecurringTransactionDetails;

public sealed class GetRecurringTransactionDetailsQueryHandler(
    ICurrentUser currentUser,
    IRecurringTransactionQueries recurringTransactionQueries,
    ILogger<GetRecurringTransactionDetailsQueryHandler> logger)
    : IRequestHandler<GetRecurringTransactionDetailsQuery, Result<RecurringTransactionListItem>>
{
    public async Task<Result<RecurringTransactionListItem>> Handle(GetRecurringTransactionDetailsQuery query,
        CancellationToken cancellationToken)
    {
        var recurring = await recurringTransactionQueries.GetRecurringTransactionDetails(query.Id, currentUser.UserId);

        if (recurring is not null) return Result<RecurringTransactionListItem>.Success(recurring);
        logger.LogWarning("Recurring Transaction with ID {RecurringId} not found", query.Id);
        return Result<RecurringTransactionListItem>.Failure("Recurring Transaction not found", ErrorType.NotFound);
    }
}