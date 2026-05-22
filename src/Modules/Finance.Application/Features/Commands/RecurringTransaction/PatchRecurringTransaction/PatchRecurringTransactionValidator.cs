using BuildingBlocks.Constants;
using FluentValidation;

namespace Finance.Application.Features.Commands.RecurringTransaction.PatchRecurringTransaction;

public class PatchRecurringTransactionValidator : AbstractValidator<PatchRecurringTransactionCommand>
{
    public PatchRecurringTransactionValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");
        
        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue)
            .WithMessage("Invalid transaction type.");  
        
        RuleFor(x => x.Description)
            .MaximumLength(RecurringTransactionConstants.MaxDescriptionLength)
            .When(_ => true)
            .WithMessage("Description cannot be longer than 250 characters.");
        
        RuleFor(x => x.Frequency)
            .IsInEnum()
            .When(x => x.Frequency.HasValue)
            .WithMessage("Invalid transaction frequency.");  
    }
}