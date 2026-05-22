using MediatR;
using SharedKernel.Common;

namespace Finance.Application.Features.Commands.RecurringTransaction.DeleteRecurringTransaction;

public record DeleteRecurringTransactionCommand(Guid Id) : IRequest<Result>;