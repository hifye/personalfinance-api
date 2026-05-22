using BuildingBlocks.Constants;
using Catalog.Domain.Enums;
using SharedKernel.Common;

namespace Catalog.Domain.Entities;

public sealed class Catalog
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public CatalogType Type { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Catalog(Guid userId, string name, CatalogType type)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Name = name;
        Type = type;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<Catalog> Create(Guid userId, string name, CatalogType type)
    {
        return Guard.AgainstOutOfRange(userId == Guid.Empty, "The User id is mandatory.")
            .Bind(() => Guard.AgainstNullOrWhiteSpace(name, "The field name is mandatory."))
            .Bind(() => name.Length > CatalogConstants.MaxNameLength
                ? Result.Failure($"The field name cannot be longer than {CatalogConstants.MaxNameLength} characters.", ErrorType.Validation)
                : Result.Success())
            .Map(() => new Catalog(userId, name, type));
    }

    public Result Patch(string? name, CatalogType? type, bool? isActive)
    {
        return Guard.AgainstOutOfRange(name == null && type == null && isActive == null,
                "At least one field must be provided for patching.")
            .Bind(() => name != null ? UpdateName(name) : Result.Success())
            .Bind(() => type != null ? UpdateType(type.Value) : Result.Success())
            .Bind(() => isActive != null ? UpdateIsActive(isActive.Value) : Result.Success());
    }

    private Result UpdateName(string name)
    {
        return Guard.AgainstOutOfRange(name.Length > CatalogConstants.MaxNameLength, $"The field name cannot be longer than {CatalogConstants.MaxNameLength} characters.")
            .Bind(() =>
            {
                Name = name;
                return Result.Success();
            });
    }

    private Result UpdateType(CatalogType type)
    {
        Type = type;
        return Result.Success();
    }

    private Result UpdateIsActive(bool isActive)
    {
        IsActive = isActive;
        return Result.Success();
    }

    protected Catalog()
    {
    }
}