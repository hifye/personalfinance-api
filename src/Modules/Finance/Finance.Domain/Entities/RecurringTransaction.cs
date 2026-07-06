using BuildingBlocks.Constants;
using Finance.Domain.Enums;
using SharedKernel.Common;
using SharedKernel.ValueObjects;

namespace Finance.Domain.Entities;

public sealed class RecurringTransaction
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid AccountId { get; private set; }
    public Guid CategoryId { get; private set; }
    public Price Amount { get; private set; } = null!;
    public TransactionType Type { get; private set; }
    public string Description { get; private set; } = null!;
    public RecurringFrequency Frequency { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public DateOnly NextOccurrence { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private RecurringTransaction(Guid userId, Guid accountId, Guid categoryId, Price amount, TransactionType type, string description, RecurringFrequency frequency)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        Id = Guid.NewGuid();
        UserId = userId;
        AccountId = accountId;
        CategoryId = categoryId;
        Amount = amount;
        Type = type;
        Description = description;
        Frequency = frequency;
        StartDate = today;
        NextOccurrence = CalculateNextOccurrence(today, frequency);
        EndDate = null;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<RecurringTransaction> Create(Guid userId, Guid accountId, Guid categoryId, decimal amount, TransactionType type, string description, RecurringFrequency frequency)
    {
        return Guard
            .AgainstOutOfRange(userId == Guid.Empty, "The field User id cannot be empty.")
            .Bind(() =>
                Guard.AgainstOutOfRange(
                    accountId == Guid.Empty,
                    "The field Account id cannot be empty."
                )
            )
            .Bind(() =>
                Guard.AgainstOutOfRange(
                    categoryId == Guid.Empty,
                    "The field Category id cannot be empty."
                )
            )
            .Bind(() =>
                Guard.AgainstOutOfRange(
                    description.Length > TransactionConstants.MaxDescriptionLength,
                    "The field Description cannot be longer than 250 characters."
                )
            )
            .Bind(() => Price.Create(amount))
            .Map(validAmount => new RecurringTransaction(
                userId,
                accountId,
                categoryId,
                validAmount,
                type,
                description,
                frequency
            ));
    }

    public Result Patch(decimal? amount, TransactionType? type, string? description, RecurringFrequency? frequency, bool? isActive)
    {
        return Guard
            .AgainstOutOfRange(
                amount == null
                    && type == null
                    && description == null
                    && frequency == null
                    && isActive == null,
                "At least one field must be provided for patching."
            )
            .Bind(() => amount != null ? UpdateAmount(amount.Value) : Result.Success())
            .Bind(() => type != null ? UpdateType(type.Value) : Result.Success())
            .Bind(() => description != null ? UpdateDescription(description) : Result.Success())
            .Bind(() => frequency != null ? UpdateFrequency(frequency.Value) : Result.Success())
            .Bind(() => isActive != null ? UpdateIsActive(isActive.Value) : Result.Success())
            .Map(() =>
            {
                UpdatedAt = DateTime.UtcNow;
                return true;
            });
    }

    private Result UpdateAmount(decimal amount)
    {
        return Price
            .Create(amount)
            .Bind(validAmount =>
            {
                Amount = validAmount;
                return Result.Success();
            });
    }

    private Result UpdateType(TransactionType type)
    {
        Type = type;
        return Result.Success();
    }

    private Result UpdateDescription(string description)
    {
        return Guard
            .AgainstOutOfRange(
                description.Length > TransactionConstants.MaxDescriptionLength,
                "The field Description cannot be longer than 250 characters."
            )
            .Bind(() =>
            {
                Description = description;
                return Result.Success();
            });
    }

    private Result UpdateFrequency(RecurringFrequency frequency)
    {
        Frequency = frequency;

        NextOccurrence = CalculateNextOccurrence(DateOnly.FromDateTime(DateTime.UtcNow), frequency);

        return Result.Success();
    }

    private Result UpdateIsActive(bool isActive)
    {
        IsActive = isActive;
        return Result.Success();
    }

    private static DateOnly CalculateNextOccurrence(
        DateOnly currentDate,
        RecurringFrequency frequency
    )
    {
        return frequency switch
        {
            RecurringFrequency.Daily => currentDate.AddDays(1),

            RecurringFrequency.Weekly => currentDate.AddDays(7),

            RecurringFrequency.Monthly => currentDate.AddMonths(1),

            RecurringFrequency.Yearly => currentDate.AddYears(1),

            _ => currentDate.AddMonths(1),
        };
    }

    protected RecurringTransaction() { }
}
