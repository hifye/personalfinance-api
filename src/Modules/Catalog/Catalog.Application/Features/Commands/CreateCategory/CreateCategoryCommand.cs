using Catalog.Domain.Enums;
using MediatR;
using SharedKernel.Common;

namespace Catalog.Application.Features.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    CatalogType Type) : IRequest<Result<Guid>>;
