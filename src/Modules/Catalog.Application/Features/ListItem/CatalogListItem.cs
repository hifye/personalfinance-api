namespace Catalog.Application.Features.ListItem;

public record CatalogListItem(
    Guid Id,
    string Name,
    string Type,
    bool IsActive,
    DateTime CreatedAt
);