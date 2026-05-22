using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Finance.Application.Features.Commands.RecurringTransaction.PatchRecurringTransaction;

public sealed class PatchRecurringTransactionCommandHandler(
    IRecurringTransactionRepository recurringTransactionRepository,
    IUnitOfWork unitOfWork, ILogger<PatchRecurringTransactionCommandHandler> logger)
    : IRequestHandler<PatchRecurringTransactionCommand, Result>
{
    public async Task<Result> Handle(PatchRecurringTransactionCommand command, CancellationToken cancellationToken)
    {
        var recurringTransaction = await recurringTransactionRepository.GetRecurringTransactionById(command.Id);
        if(recurringTransaction is null)
        {
            logger.LogWarning("Recurring Transaction with ID {RecurringTransactionId} not found", command.Id);
            return Result.Failure("Recurring Transaction not found.", ErrorType.NotFound);
        }
        
        var result = recurringTransaction.Patch(command.Amount, command.Type, command.Description, command.Frequency, command.IsActive);
        if (result.IsFailure)
            return result;
        
        await recurringTransactionRepository.UpdateRecurringTransaction(recurringTransaction);
        await unitOfWork.CommitAsync();
        return Result.Success();
    }
}
