using Finance.Application.Features.ListItem;
using MediatR;
using SharedKernel.Common;

namespace Finance.Application.Features.Queries.RecurringTransaction.GetRecurringTransactionDetails;

public record GetRecurringTransactionDetailsQuery(Guid Id) : IRequest<Result<RecurringTransactionListItem>>;