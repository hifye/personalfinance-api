using Finance.Application.Features.ListItem;
using MediatR;
using SharedKernel.Common;

namespace Finance.Application.Features.Queries.Transaction.GetTransactionDetails;

public record GetTransactionDetailsQuery(Guid Id) : IRequest<Result<TransactionListItem>>;