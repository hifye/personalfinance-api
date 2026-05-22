using System.Text.Json.Serialization;
using Catalog.Domain.Enums;
using MediatR;
using SharedKernel.Common;

namespace Catalog.Application.Features.Commands.PatchCategory;

public record PatchCategoryCommand([property: JsonIgnore] Guid Id, string? Name, CatalogType? Type, bool? IsActive)
    : IRequest<Result>;