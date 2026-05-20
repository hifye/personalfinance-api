using System.Data;
using Auth.Application.Abstractions.Persistance;
using Auth.Domain.Entities;
using Auth.Infrastructure.Persistance.Sql;
using Dapper;
using SharedKernel.ValueObjects;

namespace Auth.Infrastructure.Persistance.Repositories;

public class UserRepository(IUnitOfWork unitOfWork) : IUserRepository
{
    public async Task<User> GetUserById(Guid id) =>
        (await unitOfWork.Connection.QueryFirstOrDefaultAsync<User>(UserSql.GetUserById, new { Id = id }))!;

    public async Task<User?> GetUserByEmail(Email email) =>
        await unitOfWork.Connection.QueryFirstOrDefaultAsync<User>(
            UserSql.GetUserByEmail,
            new { Email = email }
        );

    public async Task CreateUser(User user) =>
        await unitOfWork.Connection.ExecuteAsync(
            UserSql.CreateUser,
            new
            {
                user.Name,
                user.Email,
                user.PasswordHash,
                user.CreatedAt
            },
            transaction: unitOfWork.Transaction
        );

    public async Task<bool> UpdateUser(User user) =>
        await unitOfWork.Connection.ExecuteAsync(
            UserSql.UpdateUser,
            new { user.PasswordHash, user.UpdatedAt, user.Id },
            transaction: unitOfWork.Transaction
        ) > 0;

    public async Task<bool> DeleteUser(Guid id) =>
        await unitOfWork.Connection.ExecuteAsync(UserSql.DeleteUser, new { Id = id }, transaction: unitOfWork.Transaction)
        > 0;
}