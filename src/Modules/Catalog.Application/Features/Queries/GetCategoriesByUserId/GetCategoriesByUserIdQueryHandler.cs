using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Abstractions.Queries;
using Catalog.Application.Features.ListItem;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Catalog.Application.Features.Queries.GetCategoriesByUserId;

public class GetCategoriesByUserIdQueryHandler(ICatalogQueries categoryQueries, ICurrentUser currentUser, ILogger<GetCategoriesByUserIdQueryHandler> logger)
    : IRequestHandler<GetCategoriesByUserIdQuery, Result<IReadOnlyList<CatalogListItem>>>
{
    public async Task<Result<IReadOnlyList<CatalogListItem>>> Handle(GetCategoriesByUserIdQuery query, CancellationToken cancellationToken)
    {
        var categories = await categoryQueries.GetCategoriesByUserId(currentUser.UserId);

        if (categories.Any()) return Result<IReadOnlyList<CatalogListItem>>.Success(categories);
        logger.LogWarning("No categories found for user {UserId}", currentUser.UserId);
        return Result<IReadOnlyList<CatalogListItem>>.Failure("No categories found for the user", ErrorType.NotFound);

    }
}