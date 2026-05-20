using Auth.Domain.Entities;
using SharedKernel.ValueObjects;

namespace Auth.Application.Abstractions.Persistance;

public interface IUserRepository
{
    Task<User> GetUserById(Guid id);
    Task<User?> GetUserByEmail(Email email);
    Task CreateUser(User user);
    Task<bool> UpdateUser(User user);
    Task<bool> DeleteUser(Guid id);
}