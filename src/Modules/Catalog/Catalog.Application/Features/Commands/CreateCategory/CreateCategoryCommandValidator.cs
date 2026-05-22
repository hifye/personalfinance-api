using BuildingBlocks.Constants;
using Catalog.Domain.Enums;
using FluentValidation;

namespace Catalog.Application.Features.Commands.CreateCategory;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(CatalogConstants.MaxNameLength).WithMessage($"Name cannot be longer than {CatalogConstants.MaxNameLength} characters.");
        
        RuleFor(x => x.Type)
            .NotEqual(CatalogType.None)
            .WithMessage("Invalid category type."); 
    }
}