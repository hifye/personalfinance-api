using BuildingBlocks.Constants;
using FluentValidation;

namespace Finance.Application.Features.Commands.Account.PatchAccount;

public sealed class PatchAccountCommandValidator : AbstractValidator<PatchAccountCommand>
{
    public PatchAccountCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");
        
        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue)
            .WithMessage("Invalid account type.");
    }
}
