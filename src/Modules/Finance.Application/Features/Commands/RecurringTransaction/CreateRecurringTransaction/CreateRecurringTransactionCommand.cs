using Finance.Domain.Enums;
using MediatR;
using SharedKernel.Common;

namespace Finance.Application.Features.Commands.RecurringTransaction.CreateRecurringTransaction;

public record CreateRecurringTransactionCommand(
    Guid AccountId,
    Guid CategoryId,
    decimal Amount,
    TransactionType Type,
    string Description,
    RecurringFrequency Frequency
) : IRequest<Result<Guid>>;