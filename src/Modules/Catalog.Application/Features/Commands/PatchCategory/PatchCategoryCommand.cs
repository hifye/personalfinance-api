using System.Text.Json.Serialization;
using MediatR;
using SharedKernel.Common;

namespace Catalog.Application.Features.Commands.PatchCategory;

public record PatchCategoryCommand([property: JsonIgnore] Guid Id, string? Name, string? Type, bool? IsActive)
    : IRequest<Result>;