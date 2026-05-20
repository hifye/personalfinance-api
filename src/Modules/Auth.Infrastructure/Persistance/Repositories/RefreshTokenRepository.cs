using System.Data;
using Auth.Application.Abstractions.Persistance;
using Auth.Domain.Entities;
using Auth.Infrastructure.Persistance.Connection;
using Auth.Infrastructure.Persistance.Sql;
using Dapper;

namespace Auth.Infrastructure.Persistance.Repositories;

public class RefreshTokenRepository(IDbConnectionFactory connectionFactory, IUnitOfWork unitOfWork)
    : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetRefreshTokenByHash(
        string tokenHash,
        CancellationToken cancellationToken = default
    )
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<RefreshToken>(
            RefreshTokenSql.GetRefreshToken,
            new { TokenHash = tokenHash }
        );
    }

    public async Task CreateRefreshToken(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default
    )
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            RefreshTokenSql.CreateRefreshToken,
            new
            {
                refreshToken.UserId,
                TokenHash = refreshToken.TokenHash,
                refreshToken.ExpiresAt,
                refreshToken.IsRevoked,
                refreshToken.CreatedAt,
            },
            transaction: unitOfWork.Transaction
        );
    }

    public async Task<bool> RevokeRefreshToken(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(
            RefreshTokenSql.RevokeRefreshToken,
            new { Id = id },
            transaction: unitOfWork.Transaction
        ) > 0;
    }

    public async Task RevokeAllUserTokens(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            RefreshTokenSql.RevokeAllUserTokens,
            new { UserId = userId },
            transaction: unitOfWork.Transaction
        );
    }
}