using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Finance.Application.Features.Commands.Account.DeleteAccount;

public sealed class DeleteAccountCommandHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork, ILogger<DeleteAccountCommandHandler> logger)
    : IRequestHandler<DeleteAccountCommand, Result>
{
    public async Task<Result> Handle(DeleteAccountCommand command, CancellationToken cancellationToken)
    {
        var deleted = await accountRepository.DeleteAccount(command.Id);
        if (!deleted)
        {
            logger.LogWarning("Account with ID {AccountId} not found", command.Id);
            return Result.Failure("Account not found.", ErrorType.NotFound);
        }
        
        await unitOfWork.CommitAsync();
        return Result.Success();
    }
}
