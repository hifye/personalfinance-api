using Finance.Application.Features.ListItem;
using MediatR;
using SharedKernel.Common;

namespace Finance.Application.Features.Queries.RecurringTransaction.GetRecurringTransactionsByUserId;

public record GetRecurringTransactionsByUserIdQuery : IRequest<Result<IReadOnlyList<RecurringTransactionListItem>>>;