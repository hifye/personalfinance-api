using Finance.Domain.Entities;

namespace Finance.Application.Abstractions.Persistance;

public interface IRecurringTransactionRepository
{
    Task<RecurringTransaction?> GetRecurringTransactionById(Guid id);
    Task CreateRecurringTransaction(RecurringTransaction recurringTransaction);
    Task<bool> UpdateRecurringTransaction(RecurringTransaction recurringTransaction);
    Task<bool> DeleteRecurringTransaction(Guid id);
}