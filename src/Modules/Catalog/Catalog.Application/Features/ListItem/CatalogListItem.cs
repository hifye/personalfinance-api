using Catalog.Domain.Enums;

namespace Catalog.Application.Features.ListItem;

public record CatalogListItem(
    Guid Id,
    string Name,
    CatalogType Type,
    bool IsActive,
    DateTime CreatedAt
);