using FluentValidation;

namespace Catalog.Application.Features.Commands.PatchCategory;

public class PatchCategoryCommandValidator : AbstractValidator<PatchCategoryCommand>
{
    public PatchCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
        
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name cannot be longer than 100 characters.");
        
        RuleFor(x => x.Type)
            .MaximumLength(100).WithMessage("Type cannot be longer than 100 characters.");
    }
}