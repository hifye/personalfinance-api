using System.Data;
using Auth.Application.Abstractions.Persistance;
using Auth.Domain.Entities;
using Auth.Infrastructure.Data.Sql;
using Dapper;
using SharedKernel.ValueObjects;

namespace Auth.Infrastructure.Persistance.Repositories;

public class UserRepository(IUnitOfWork unitOfWork, IDbConnection connection) : IUserRepository
{
    public async Task<User> GetUserById(Guid id) =>
        (await connection.QueryFirstOrDefaultAsync<User>(UserSql.GetUserById, new { Id = id }))!;

    public async Task<User?> GetUserByEmail(Email email) =>
        await connection.QueryFirstOrDefaultAsync<User>(
            UserSql.GetUserByEmail,
            new { Email = email }
        );

    public async Task CreateUser(User user) =>
        await connection.ExecuteAsync(
            UserSql.CreateUser,
            new
            {
                user.Name,
                user.Email,
                user.PasswordHash,
                user.CreatedAt
            },
            unitOfWork.Transaction
        );

    public async Task<bool> UpdateUser(User user) =>
        await connection.ExecuteAsync(
            UserSql.UpdateUser,
            new { user.PasswordHash, user.UpdatedAt, user.Id },
            unitOfWork.Transaction
        ) > 0;

    public async Task<bool> DeleteUser(Guid id) =>
        await connection.ExecuteAsync(UserSql.DeleteUser, new { Id = id }, unitOfWork.Transaction)
        > 0;
}