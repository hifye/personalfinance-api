namespace Auth.Infrastructure.Persistance.Sql;

public static class RefreshTokenSql
{
    public const string GetRefreshToken = """
                                          SELECT id                         AS Id,
                                                 user_id                    AS UserId,
                                                 token_hash                 AS TokenHash,
                                                 expires_at                 AS ExpiresAt,
                                                 is_revoked                 AS IsRevoked,
                                                 created_at                 AS CreatedAt
                                          FROM auth.refresh_tokens
                                          WHERE token_hash = @TokenHash AND is_revoked = false AND expires_at > NOW()
                                          """;

    public const string CreateRefreshToken = """
                                             INSERT INTO auth.refresh_tokens 
                                                 (user_id, 
                                                  token_hash, 
                                                  expires_at,
                                                  is_revoked, 
                                                  created_at)
                                             VALUES (
                                                  (@UserId), 
                                                  (@TokenHash),
                                                  (@ExpiresAt), 
                                                  (@IsRevoked), 
                                                  (@CreatedAt))
                                             """;

    public const string RevokeRefreshToken = """
                                             UPDATE auth.refresh_tokens
                                             SET is_revoked = true
                                             WHERE id = @Id
                                             """;

    public const string RevokeAllUserTokens = """
                                              UPDATE auth.refresh_tokens
                                              SET is_revoked = true
                                              WHERE user_id = @UserId
                                              """;
}
