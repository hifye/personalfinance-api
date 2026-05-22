using Finance.Application.Features.ListItem;
using MediatR;
using SharedKernel.Common;

namespace Finance.Application.Features.Queries.Transaction.GetTransactionSummary;

public record GetTransactionSummaryQuery(DateTime StartDate, DateTime EndDate) : IRequest<Result<TransactionSummary>>;