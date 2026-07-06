using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Contracts;
using Finance.Application.Abstractions.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Finance.Application.Features.Commands.RecurringTransaction.CreateRecurringTransaction;

public sealed class CreateRecurringTransactionCommandHandler(
    IRecurringTransactionRepository recurringTransactionRepository,
    IUnitOfWork unitOfWork,
    ICatalogModule catalogModule,
    IAccountRepository accountRepository,
    ICurrentUser currentUser,
    ILogger<CreateRecurringTransactionCommandHandler> logger)
    : IRequestHandler<CreateRecurringTransactionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateRecurringTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetAccountById(command.AccountId, currentUser.UserId);
        if (account is null)
        {
            logger.LogWarning("Account with ID {AccountId} not found", command.AccountId);
            return Result<Guid>.Failure("Account not found", ErrorType.NotFound);
        }


        var categoryExists =
            await catalogModule.CategoryExistsAsync(command.CategoryId, currentUser.UserId, cancellationToken);
        if (!categoryExists)
        {
            logger.LogWarning("Category with ID {CategoryId} not found", command.CategoryId);
            return Result<Guid>.Failure("Category not found", ErrorType.NotFound);
        }

        return await Domain.Entities.RecurringTransaction
            .Create(currentUser.UserId,
                command.AccountId,
                command.CategoryId,
                command.Amount,
                command.Type,
                command.Description,
                command.Frequency)
            .BindAsync(async recurringTransaction =>
            {
                await recurringTransactionRepository.CreateRecurringTransaction(recurringTransaction);
                await unitOfWork.CommitAsync();
                return Result<Guid>.Success(recurringTransaction.Id);
            });
    }
}