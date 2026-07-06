using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Finance.Application.Features.Commands.Account.PatchAccount;

public sealed class PatchAccountCommandHandler(
    ICurrentUser currentUser,
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork,
    ILogger<PatchAccountCommandHandler> logger)
    : IRequestHandler<PatchAccountCommand, Result>
{
    public async Task<Result> Handle(PatchAccountCommand command, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetAccountById(command.Id, currentUser.UserId);
        if (account is null)
        {
            logger.LogWarning("Account with ID {AccountId} not found", command.Id);
            return Result.Failure("Account not found.", ErrorType.NotFound);
        }

        var result = account.Patch(command.Type, command.IsActive);
        if (result.IsFailure)
        {
            logger.LogWarning("Failed to patch account {AccountId}", command.Id);
            return result;
        }

        await accountRepository.UpdateAccount(account);
        await unitOfWork.CommitAsync();
        return Result.Success();
    }
}