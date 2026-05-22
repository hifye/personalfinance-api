using System;
using System.Data;
using System.Threading.Tasks;
using Auth.Application.Abstractions.Persistance;
using Auth.Domain.Entities;
using Auth.Infrastructure.Persistance.Sql;
using BuildingBlocks.Application.Abstractions;
using Dapper;
using SharedKernel.ValueObjects;

namespace Auth.Infrastructure.Persistance.Repositories;

public sealed class UserRepository(IDbConnectionFactory connectionFactory, IUnitOfWork unitOfWork)
    : IUserRepository
{
    public async Task<User> GetUserById(Guid id)
    {
        using var connection = connectionFactory.CreateConnection();
        return (
            await connection.QueryFirstOrDefaultAsync<User>(UserSql.GetUserById, new { Id = id })
        )!;
    }

    public async Task<User?> GetUserByEmail(Email email)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(
            UserSql.GetUserByEmail,
            new { Email = email }
        );
    }

    public async Task CreateUser(User user)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            UserSql.CreateUser,
            new
            {
                user.Name,
                user.Email,
                user.PasswordHash,
                user.CreatedAt,
            },
            transaction: unitOfWork.Transaction
        );
    }

    public async Task<bool> UpdateUser(User user)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(
            UserSql.UpdatePassword,
            new
            {
                user.PasswordHash,
                user.UpdatedAt,
                user.Id,
            },
            transaction: unitOfWork.Transaction
        ) > 0;
    }

    public async Task<bool> DeleteUser(Guid id)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(
            UserSql.DeleteUser,
            new { Id = id },
            transaction: unitOfWork.Transaction
        ) > 0;
    }
}