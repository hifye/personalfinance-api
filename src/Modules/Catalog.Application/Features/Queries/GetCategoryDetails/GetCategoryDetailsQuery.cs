using Catalog.Application.Features.ListItem;
using MediatR;
using SharedKernel.Common;

namespace Catalog.Application.Features.Queries.GetCategoryDetails;

public record GetCategoryDetailsQuery(Guid Id) : IRequest<Result<CatalogListItem>>;