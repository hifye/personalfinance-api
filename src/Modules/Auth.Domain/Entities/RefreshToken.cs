using SharedKernel.Common;

namespace Auth.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private RefreshToken(Guid userId, string token, DateTime expiresAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        TokenHash = token;
        ExpiresAt = expiresAt;
        IsRevoked = false;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<RefreshToken> Create(Guid userId, string tokenHash, DateTime expiresAt)
    {
        return Guard.AgainstOutOfRange(userId == Guid.Empty, "The User id cannot be empty")
            .Bind(() => Guard.AgainstNullOrWhiteSpace(tokenHash, "Token cannot be empty"))
            .Bind(() => Guard.AgainstOutOfRange(expiresAt <= DateTime.UtcNow, "Expiration date cannot be in the past"))
            .Map(() => new RefreshToken(userId, tokenHash, expiresAt));
    }

    public Result Revoke()
    {
        return Guard.AgainstOutOfRange(IsRevoked, "The token has already been revoked")
            .Bind(() =>
            {
                IsRevoked = true;
                return Result.Success();
            });
    }
    
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
    
    protected RefreshToken() { }
}