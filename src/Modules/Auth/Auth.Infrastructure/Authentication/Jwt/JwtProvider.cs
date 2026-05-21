using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Auth.Application.Abstractions.Authentication;
using Auth.Application.Authentication.Responses;
using Auth.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Infrastructure.Authentication.Jwt;

internal sealed class JwtProvider(IOptions<JwtSettings> jwtSettings) : IJwtProvider
{
    public TokenResponse Generate(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email.Address),
            new Claim(ClaimTypes.Name, user.Name),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.Key));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddDays(jwtSettings.Value.RefreshTokenExpirationDays);

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Value.Issuer,
            audience: jwtSettings.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.Value.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshToken = GenerateRefreshToken();

        return new TokenResponse(accessToken, refreshToken, expiresAt);
    }

    private static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);

        return Convert.ToBase64String(bytes);
    }
}
