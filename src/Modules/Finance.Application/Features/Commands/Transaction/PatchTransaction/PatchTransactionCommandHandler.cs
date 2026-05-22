using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Finance.Application.Features.Commands.Transaction.PatchTransaction;

public sealed class PatchTransactionCommandHandler(ITransactionRepository transactionRepository, IUnitOfWork unitOfWork, ILogger<PatchTransactionCommandHandler> logger)
    : IRequestHandler<PatchTransactionCommand, Result>
{
    public async Task<Result> Handle(PatchTransactionCommand command, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetTransactionById(command.Id);
        if(transaction is null)
        {
            logger.LogWarning("Transaction with ID {TransactionId} not found", command.Id);
            return Result.Failure("Transaction not found.", ErrorType.NotFound);
        }
        
        var result = transaction.Patch(command.Type, command.Description);
        if (result.IsFailure)
        {
            logger.LogWarning("Failed to patch transaction {TransactionId}", command.Id);
            return result;
        }
            
        
        await transactionRepository.UpdateTransaction(transaction);
        await unitOfWork.CommitAsync();
        return Result.Success();
    }
}
