using Finance.Domain.Enums;
using SharedKernel.Common;
using SharedKernel.ValueObjects;

namespace Finance.Domain.Entities;

public sealed class Account
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public AccountType Type { get; private set; }
    public Price InitialBalance { get; private set; } = null!;
    public Price CurrentBalance { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Account(Guid userId, string name, AccountType type, Price initialBalance)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Name = name;
        Type = type;
        InitialBalance = initialBalance;
        CurrentBalance = initialBalance;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static Result<Account> Create(Guid userId, string name, AccountType type, decimal initialBalance)
    {
        return Guard.AgainstOutOfRange(userId == Guid.Empty, "The field User id is mandatory.")
            .Bind(() => Guard.AgainstNullOrWhiteSpace(name, "The field name is mandatory."))
            .Bind(() => name.Length > 50
                ? Result.Failure("The name cannot be longer than 50 characters.", ErrorType.Validation)
                : Result.Success())
            .Bind(() => Price.Create(initialBalance))
            .Map(validPrice => new Account(userId, name, type, validPrice));
    }

    public Result Patch(AccountType? type, bool? isActive)
    {
        return Guard.AgainstOutOfRange(type == null && isActive == null,
                "At least one field must be provided for patching.")
            .Bind(() => type != null ? UpdateType(type.Value) : Result.Success())
            .Bind(() => isActive != null ? UpdateActive(isActive.Value) : Result.Success());
    }

    private Result UpdateType(AccountType type)
    {
        Type = type;
        return Result.Success();
    }

    private Result UpdateActive(bool isActive)
    {
        IsActive = isActive;
        return Result.Success();
    }

    protected Account()
    {
    }
}