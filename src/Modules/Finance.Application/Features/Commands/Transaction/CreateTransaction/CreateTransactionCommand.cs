using Finance.Domain.Enums;
using MediatR;
using SharedKernel.Common;

namespace Finance.Application.Features.Commands.Transaction.CreateTransaction;

public record CreateTransactionCommand(
    Guid AccountId,
    Guid CategoryId,
    Guid? RecurringId,
    decimal Amount,
    TransactionType Type,
    string Description
) : IRequest<Result<Guid>>;
