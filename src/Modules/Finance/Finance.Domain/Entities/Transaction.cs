using Finance.Domain.Enums;
using SharedKernel.Common;
using SharedKernel.ValueObjects;

namespace Finance.Domain.Entities;

public sealed class Transaction
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid AccountId { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid? RecurringId { get; private set; }
    public Price Amount { get; private set; } = null!;
    public TransactionType Type { get; private set; }
    public string Description { get; private set; } = null!;
    public DateOnly TransactionDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Transaction(Guid userId, Guid accountId, Guid categoryId, Guid? recurringId, Price amount,
        TransactionType type, string description)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        AccountId = accountId;
        CategoryId = categoryId;
        RecurringId = recurringId;
        Amount = amount;
        Type = type;
        Description = description;
        TransactionDate = DateOnly.FromDateTime(DateTime.UtcNow);
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<Transaction> Create(Guid userId, Guid accountId, Guid categoryId, Guid? recurringId,
        decimal amount, TransactionType type, string description)
    {
        return Guard.AgainstOutOfRange(
                userId == Guid.Empty,
                "The field User id cannot be empty")
            .Bind(() => Guard.AgainstOutOfRange(
                accountId == Guid.Empty,
                "The field Account id cannot be empty"))
            .Bind(() => Guard.AgainstOutOfRange(
                categoryId == Guid.Empty,
                "The field Category id cannot be empty"))
            .Bind(() => Guard.AgainstOutOfRange(
                description.Length > 250,
                "The field Description cannot be longer than 250 characters."))
            .Bind(() => Price.Create(amount))
            .Map(validPrice =>
                new Transaction(
                    userId,
                    accountId,
                    categoryId,
                    recurringId,
                    validPrice,
                    type,
                    description));
    }

    public Result Patch(TransactionType? type, string? description)
    {
        return Guard.AgainstOutOfRange(type == null && description == null, "At least one field must be provided for patching.")
            .Bind(() => type != null
                ? UpdateType(type.Value)
                : Result.Success())
            .Bind(() => description != null
                ? UpdateDescription(description)
                : Result.Success())
            .Map(() =>
            {
                UpdatedAt = DateTime.UtcNow;
                return true;
            });
    }

    private Result UpdateType(TransactionType type)
    {
        Type = type;
        return Result.Success();
    }

    private Result UpdateDescription(string description)
    {
        return Guard.AgainstOutOfRange(
                description.Length > 250,
                "The field Description cannot be longer than 250 characters.")
            .Bind(() =>
            {
                Description = description;
                return Result.Success();
            });
    }

    protected Transaction()
    {
    }
}