using Auth.Domain.Entities;

namespace Auth.Application.Abstractions.Persistance;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetRefreshToken(string token, CancellationToken cancellationToken = default);
    Task CreateRefreshToken(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task<bool> RevokeRefreshToken(Guid id, CancellationToken cancellationToken = default);
    Task RevokeAllUserTokens(Guid userId, CancellationToken cancellationToken = default);
}