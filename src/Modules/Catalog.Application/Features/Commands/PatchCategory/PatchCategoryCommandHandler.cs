using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Abstractions.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Catalog.Application.Features.Commands.PatchCategory;

public class PatchCategoryCommandHandler(IUnitOfWork unitOfWork, ICatalogRepository catalogRepository, ILogger<PatchCategoryCommandHandler> logger)
    : IRequestHandler<PatchCategoryCommand, Result>
{
    public async Task<Result> Handle(PatchCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await catalogRepository.GetCategoryById(command.Id);
        if(category is null)
        {
            logger.LogWarning("Category not found for id {CategoryId}", command.Id);
            return Result.Failure("Category not found.", ErrorType.NotFound);
        }
        
        var result = category.Patch(command.Name, command.Type, command.IsActive);
        if (result.IsFailure)
        {
            logger.LogWarning("Failed to update category {CategoryId}", command.Id);
            return result;
        }
        
        await catalogRepository.PatchCategory(category);
        await unitOfWork.CommitAsync();
        return Result.Success();
    }
}