using Auth.Domain.Entities;

namespace Auth.Application.Abstractions.Authentication;

public interface IJwtProvider
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}