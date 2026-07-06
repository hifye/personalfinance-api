using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Abstractions.Queries;
using Catalog.Application.Features.ListItem;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Catalog.Application.Features.Queries.GetCategoryDetails;

public sealed class GetCategoryDetailsQueryHandler(
    ICurrentUser currentUser,
    ICatalogQueries catalogQueries,
    ILogger<GetCategoryDetailsQueryHandler> logger)
    : IRequestHandler<GetCategoryDetailsQuery, Result<CatalogListItem>>
{
    public async Task<Result<CatalogListItem>> Handle(GetCategoryDetailsQuery query,
        CancellationToken cancellationToken)
    {
        var category = await catalogQueries.GetCategoryDetails(query.Id, currentUser.UserId);

        if (category is not null) return Result<CatalogListItem>.Success(category);
        logger.LogWarning("Category with ID {CategoryId} not found", query.Id);
        return Result<CatalogListItem>.Failure("Category not found", ErrorType.NotFound);
    }
}