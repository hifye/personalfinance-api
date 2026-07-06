using Finance.Domain.Entities;

namespace Finance.Application.Abstractions.Persistance;

public interface IRecurringTransactionRepository
{
    Task<RecurringTransaction?> GetRecurringTransactionById(Guid id, Guid userId);
    Task CreateRecurringTransaction(RecurringTransaction recurringTransaction);
    Task<bool> UpdateRecurringTransaction(RecurringTransaction recurringTransaction);
    Task<bool> DeleteRecurringTransaction(Guid id, Guid userId);
}