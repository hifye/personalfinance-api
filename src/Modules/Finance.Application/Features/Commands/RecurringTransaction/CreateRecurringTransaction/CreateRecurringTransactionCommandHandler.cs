using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Abstractions.Persistance;
using Finance.Application.Abstractions.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Finance.Application.Features.Commands.RecurringTransaction.CreateRecurringTransaction;

public sealed class CreateRecurringTransactionCommandHandler(
    IRecurringTransactionRepository recurringTransactionRepository,
    IUnitOfWork unitOfWork,
    ICatalogRepository catalogRepository,
    IAccountRepository accountRepository,
    ICurrentUser currentUser, ILogger<CreateRecurringTransactionCommandHandler> logger)
    : IRequestHandler<CreateRecurringTransactionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateRecurringTransactionCommand command, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetAccountById(command.AccountId);
        if (account is null)
        {
            logger.LogWarning("Account with ID {AccountId} not found", command.AccountId);
            return Result<Guid>.Failure("Account not found", ErrorType.NotFound);
        }
            
        
        var category = await catalogRepository.GetCategoryById(command.CategoryId);
        if (category is null)
        {
            logger.LogWarning("Category with ID {CategoryId} not found", command.CategoryId);
            return Result<Guid>.Failure("Category Not Found", ErrorType.NotFound);
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
