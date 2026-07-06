using Finance.Domain.Entities;

namespace Finance.Application.Abstractions.Persistance;

public interface IAccountRepository
{
    Task<Account?> GetAccountById(Guid id, Guid userId);
    Task<int> CreateAccount(Account account);
    Task<bool> UpdateAccount(Account account);
    Task<bool> DeleteAccount(Guid id, Guid userId);
}