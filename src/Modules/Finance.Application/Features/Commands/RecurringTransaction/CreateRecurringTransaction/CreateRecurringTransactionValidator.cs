using BuildingBlocks.Constants;
using Finance.Domain.Enums;
using FluentValidation;

namespace Finance.Application.Features.Commands.RecurringTransaction.CreateRecurringTransaction;

public class CreateRecurringTransactionValidator : AbstractValidator<CreateRecurringTransactionCommand>
{
    public CreateRecurringTransactionValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("Account id is required.");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category id is required.");
        
        RuleFor(x => x.Amount)
            .NotEmpty()
            .WithMessage("Amount is required.");
        
        RuleFor(x => x.Type)
            .IsInEnum()
            .NotEqual(TransactionType.None)
            .WithMessage("Invalid transaction type.");  
        
        RuleFor(x => x.Description)
            .MaximumLength(RecurringTransactionConstants.MaxDescriptionLength)
            .When(_ => true)
            .WithMessage("Description cannot be longer than 250 characters.");
        
        RuleFor(x => x.Frequency)
            .IsInEnum()
            .NotEqual(RecurringFrequency.None)
            .WithMessage("Invalid recurring frequency.");  
    }
}