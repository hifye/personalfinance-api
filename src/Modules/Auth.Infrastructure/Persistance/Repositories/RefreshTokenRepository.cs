using System.Data;
using Auth.Application.Abstractions.Persistance;
using Auth.Domain.Entities;
using Auth.Infrastructure.Persistance.Sql;
using Dapper;

namespace Auth.Infrastructure.Persistance.Repositories;

public class RefreshTokenRepository(IUnitOfWork unitOfWork)
    : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetRefreshTokenByHash(
        string tokenHash,
        CancellationToken cancellationToken = default
    ) =>
        await unitOfWork.Connection.QueryFirstOrDefaultAsync<RefreshToken>(
            RefreshTokenSql.GetRefreshToken,
            new { Token = tokenHash }
        );

    public async Task CreateRefreshToken(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default
    ) =>
        await unitOfWork.Connection.ExecuteAsync(
            RefreshTokenSql.CreateRefreshToken,
            new
            {
                refreshToken.UserId,
                Token = refreshToken.TokenHash,
                refreshToken.ExpiresAt,
                refreshToken.IsRevoked,
                refreshToken.CreatedAt,
            },
            transaction: unitOfWork.Transaction
        );

    public async Task<bool> RevokeRefreshToken(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
        await unitOfWork.Connection.ExecuteAsync(
            RefreshTokenSql.RevokeRefreshToken,
            new { Id = id },
            transaction: unitOfWork.Transaction
        ) > 0;

    public async Task RevokeAllUserTokens(
        Guid userId,
        CancellationToken cancellationToken = default
    ) =>
        await unitOfWork.Connection.ExecuteAsync(
            RefreshTokenSql.RevokeAllUserTokens,
            new { UserId = userId },
            transaction: unitOfWork.Transaction
        );
}