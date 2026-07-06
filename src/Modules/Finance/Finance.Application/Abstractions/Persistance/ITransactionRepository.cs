using Finance.Domain.Entities;

namespace Finance.Application.Abstractions.Persistance;

public interface ITransactionRepository
{
    Task<Transaction?> GetTransactionById(Guid id, Guid userId);
    Task CreateTransaction(Transaction transaction);
    Task<bool> UpdateTransaction(Transaction transaction);
    Task<bool> DeleteTransaction(Guid id, Guid userId);
}