using System.Data;
using Auth.Application.Abstractions.Persistance;
using Auth.Domain.Entities;
using Auth.Infrastructure.Data.Sql;
using Dapper;

namespace Auth.Infrastructure.Persistance.Repositories;

public class RefreshTokenRepository(IUnitOfWork unitOfWork, IDbConnection connection)
    : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetRefreshToken(
        string token,
        CancellationToken cancellationToken = default
    ) =>
        await connection.QueryFirstOrDefaultAsync<RefreshToken>(
            RefreshTokenSql.GetRefreshToken,
            new { Token = token }
        );

    public async Task CreateRefreshToken(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default
    ) =>
        await connection.ExecuteAsync(
            RefreshTokenSql.CreateRefreshToken,
            new
            {
                refreshToken.UserId,
                refreshToken.Token,
                refreshToken.ExpiresAt,
                refreshToken.IsRevoked,
                refreshToken.CreatedAt,
            },
            unitOfWork.Transaction
        );

    public async Task<bool> RevokeRefreshToken(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
        await connection.ExecuteAsync(
            RefreshTokenSql.RevokeRefreshToken,
            new { Id = id },
            unitOfWork.Transaction
        ) > 0;

    public async Task RevokeAllUserTokens(
        Guid userId,
        CancellationToken cancellationToken = default
    ) =>
        await connection.ExecuteAsync(
            RefreshTokenSql.RevokeAllUserTokens,
            new { UserId = userId },
            unitOfWork.Transaction
        );
}