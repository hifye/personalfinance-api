using Finance.Application.Features.ListItem;
using MediatR;
using SharedKernel.Common;

namespace Finance.Application.Features.Queries.Transaction.GetTransactionsByUserId;

public record GetTransactionsByUserIdQuery : IRequest<Result<IReadOnlyList<TransactionListItem>>>;