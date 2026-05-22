using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Finance.Application.Features.Commands.RecurringTransaction.DeleteRecurringTransaction;

public class DeleteRecurringTransactionCommandHandler(
    IRecurringTransactionRepository recurringTransactionRepository,
    IUnitOfWork unitOfWork, ILogger<DeleteRecurringTransactionCommandHandler> logger)
    : IRequestHandler<DeleteRecurringTransactionCommand, Result>
{
    public async Task<Result> Handle(DeleteRecurringTransactionCommand command, CancellationToken cancellationToken)
    {
        var deleted = await recurringTransactionRepository.DeleteRecurringTransaction(command.Id);
        if (!deleted)
        {
            logger.LogWarning("Recurring Transaction with ID {RecurringTransactionId} not found", command.Id);
            return Result.Failure("Recurring Transaction not found.", ErrorType.NotFound);
        }
            
        await unitOfWork.CommitAsync();
        return Result.Success();
    }
}