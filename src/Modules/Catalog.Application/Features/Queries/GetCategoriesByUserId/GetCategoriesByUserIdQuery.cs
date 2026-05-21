using Catalog.Application.Features.ListItem;
using MediatR;
using SharedKernel.Common;

namespace Catalog.Application.Features.Queries.GetCategoriesByUserId;

public record GetCategoriesByUserIdQuery : IRequest<Result<IReadOnlyList<CatalogListItem>>>;