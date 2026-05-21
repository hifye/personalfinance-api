using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Abstractions.Persistance;
using MediatR;
using SharedKernel.Common;

namespace Catalog.Application.Features.Commands.CreateCategory;

public class CreateCategoryCommandHandler(ICatalogRepository catalogRepository, IUnitOfWork unitOfWork, ICurrentUser currentUser) : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        return await Domain.Entities.Catalog.Create(currentUser.UserId, command.Name, command.Type)
            .BindAsync(async category =>
            {
                await catalogRepository.CreateCategory(category);
                await unitOfWork.CommitAsync();
                return Result<Guid>.Success(category.Id);
            });
    }
}