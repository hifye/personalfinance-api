using System.Text;
using Auth.Application.Abstractions.Authentication;
using Auth.Application.Abstractions.Persistance;
using Auth.Application.Abstractions.Security;
using Auth.Infrastructure.Authentication.Jwt;
using Auth.Infrastructure.Identity;
using Auth.Infrastructure.Persistance.Connection;
using Auth.Infrastructure.Persistance.Repositories;
using Auth.Infrastructure.Persistance.UnitOfWork;
using Auth.Infrastructure.Security;
using BuildingBlocks.Application.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Persistência
        services.AddScoped<IDbConnectionFactory>(_ =>
            new DbConnectionFactory(configuration));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Segurança
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IRefreshTokenHasher, RefreshTokenHasher>();

        // Identidade
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        // JWT
        services.Configure<JwtSettings>(configuration.GetSection("JWT"));
        var jwtSettings = configuration.GetSection("JWT").Get<JwtSettings>()!;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.SaveToken = true;
                opt.RequireHttpsMetadata = false;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
            });

        return services;
    }
}