using BuildingBlocks.Constants;
using FluentValidation;

namespace Finance.Application.Features.Commands.Transaction.PatchTransaction;

public class PatchTransactionCommandValidator : AbstractValidator<PatchTransactionCommand>
{
    public PatchTransactionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");
        
        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue)
            .WithMessage("Invalid transaction type.");  
        
        RuleFor(x => x.Description)
            .MaximumLength(TransactionConstants.MaxDescriptionLength).WithMessage("Description cannot be longer than 250 characters.");       
    }
}