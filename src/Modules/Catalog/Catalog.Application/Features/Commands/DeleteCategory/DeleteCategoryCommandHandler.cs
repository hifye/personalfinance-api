using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Abstractions.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Catalog.Application.Features.Commands.DeleteCategory;

public sealed class DeleteCategoryCommandHandler(
    ICurrentUser currentUser,
    ICatalogRepository catalogRepository,
    IUnitOfWork unitOfWork,
    ILogger<DeleteCategoryCommandHandler> logger)
    : IRequestHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        var deleted = await catalogRepository.DeleteCategory(command.Id, currentUser.UserId);
        if (!deleted)
        {
            logger.LogWarning("Category not found for id {CategoryId}", command.Id);
            return Result.Failure("Category not found.", ErrorType.NotFound);
        }

        await unitOfWork.CommitAsync();
        return Result.Success();
    }
}