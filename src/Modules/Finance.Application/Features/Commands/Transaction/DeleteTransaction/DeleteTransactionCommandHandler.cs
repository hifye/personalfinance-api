using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Finance.Application.Features.Commands.Transaction.DeleteTransaction;
    
public sealed class DeleteTransactionCommandHandler(ITransactionRepository transactionRepository, IUnitOfWork unitOfWork, ILogger<DeleteTransactionCommandHandler> logger)
    : IRequestHandler<DeleteTransactionCommand, Result>
{
    public async Task<Result> Handle(DeleteTransactionCommand command, CancellationToken cancellationToken)
    {
        var deleted = await transactionRepository.DeleteTransaction(command.Id);
        if (!deleted)
        {
            logger.LogWarning("Transaction with ID {TransactionId} not found", command.Id);
            return Result.Failure("Transaction not found.", ErrorType.NotFound);
        }
            
        await unitOfWork.CommitAsync();
        return Result.Success();
    }
}
