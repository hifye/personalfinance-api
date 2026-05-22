using BuildingBlocks.Constants;
using FluentValidation;

namespace Catalog.Application.Features.Commands.PatchCategory;

public class PatchCategoryCommandValidator : AbstractValidator<PatchCategoryCommand>
{
    public PatchCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
        
        RuleFor(x => x.Name)
            .MaximumLength(CatalogConstants.MaxNameLength).WithMessage($"Name cannot be longer than {CatalogConstants.MaxNameLength} characters.");
        
        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue)
            .WithMessage("Invalid category type.");
    }
}