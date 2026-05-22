namespace Finance.Infrastructure.Persistance.Queries.Models;

public sealed class RecurringTransactionDbModel
{
    public Guid Id { get; init; }
    public Guid AccountId { get; init; }
    public Guid CategoryId { get; init; }
    public decimal Amount { get; init; }
    public short Type { get; init; }
    public string Description { get; init; } = null!;
    public short Frequency { get; init; }
    public DateOnly NextOccurrence { get; init; }
    public DateOnly? EndDate { get; init; }
    public bool IsActive { get; init; }
}