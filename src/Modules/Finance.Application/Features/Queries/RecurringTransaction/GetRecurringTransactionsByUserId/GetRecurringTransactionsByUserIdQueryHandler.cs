using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Queries;
using Finance.Application.Features.ListItem;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Finance.Application.Features.Queries.RecurringTransaction.GetRecurringTransactionsByUserId;

public class GetRecurringTransactionsByUserIdQueryHandler(
    IRecurringTransactionQueries recurringTransactionQueries,
    ICurrentUser currentUser,
    ILogger<GetRecurringTransactionsByUserIdQueryHandler> logger)
    : IRequestHandler<GetRecurringTransactionsByUserIdQuery, Result<IReadOnlyList<RecurringTransactionListItem>>>
{
    public async Task<Result<IReadOnlyList<RecurringTransactionListItem>>> Handle(
        GetRecurringTransactionsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var recurring = await recurringTransactionQueries.GetRecurringTransactionsByUserId(currentUser.UserId);

        if (recurring.Any()) return Result<IReadOnlyList<RecurringTransactionListItem>>.Success(recurring);
        logger.LogWarning("No recurring transactions found for user with ID {UserId}", currentUser.UserId);
        return Result<IReadOnlyList<RecurringTransactionListItem>>.Failure(
            "No recurring transactions found for the user", ErrorType.NotFound);
    }
}