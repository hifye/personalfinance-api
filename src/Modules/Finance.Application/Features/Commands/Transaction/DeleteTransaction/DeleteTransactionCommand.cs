using MediatR;
using SharedKernel.Common;

namespace Finance.Application.Features.Commands.Transaction.DeleteTransaction;

public record DeleteTransactionCommand(Guid Id) : IRequest<Result>;