using BuildingBlocks.Constants;
using Finance.Domain.Enums;
using FluentValidation;

namespace Finance.Application.Features.Commands.Account.CreateAccount;

public class CreateAccountValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Account name cannot be empty")
            .MaximumLength(AccountConstants.MaxNameLength)
            .WithMessage("Account name cannot be longer than 50 characters");
        
        RuleFor(x => x.Type)
            .IsInEnum()
            .NotEqual(AccountType.None)
            .WithMessage("Invalid transaction type.");  
        
        RuleFor(x => x.InitialBalance)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Initial balance cannot be negative");
    }
}