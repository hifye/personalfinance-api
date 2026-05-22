using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Abstractions.Persistance;
using Finance.Application.Abstractions.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Finance.Application.Features.Commands.Transaction.CreateTransaction;

public sealed class CreateTransactionCommandHandler(
    IRecurringTransactionRepository recurringTransactionRepository,
    ITransactionRepository transactionRepository,
    ICatalogRepository catalogRepository,
    IAccountRepository accountRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    ILogger<CreateTransactionCommandHandler> logger
) : IRequestHandler<CreateTransactionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateTransactionCommand command,
        CancellationToken cancellationToken
    )
    {
        var category = await catalogRepository.GetCategoryById(command.CategoryId);
        if (category is null)
        {
            logger.LogWarning("Category with ID {CategoryId} not found", command.CategoryId);
            return Result<Guid>.Failure("Category not found.", ErrorType.NotFound);
        }

        var account = await accountRepository.GetAccountById(command.AccountId);
        if (account is null)
        {
            logger.LogWarning("Account with ID {AccountId} not found", command.AccountId);
            return Result<Guid>.Failure("Account not found.", ErrorType.NotFound);
        }

        if (command.RecurringId.HasValue)
        {
            var recurringTransaction = await recurringTransactionRepository.GetRecurringTransactionById(
                command.RecurringId.Value
            );
            if (recurringTransaction is null)
            {
                logger.LogWarning("Recurring transaction with ID {RecurringId} not found", command.RecurringId);
                return Result<Guid>.Failure("Recurring transaction not found.", ErrorType.NotFound);
            }
        }

        return await Domain
            .Entities.Transaction.Create(
                currentUser.UserId,
                command.AccountId,
                command.CategoryId,
                command.RecurringId,
                command.Amount,
                command.Type,
                command.Description
            )
            .BindAsync(async transaction =>
            {
                await transactionRepository.CreateTransaction(transaction);
                await unitOfWork.CommitAsync();
                return Result<Guid>.Success(transaction.Id);
            });
    }
}

