using Auth.Application.Abstractions.Authentication;
using Auth.Application.Abstractions.Persistance;
using Auth.Application.Authentication.Responses;
using Auth.Application.Settings;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Common;
using SharedKernel.ValueObjects;

namespace Auth.Application.Authentication.Login;

internal sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider,
    ILogger<LoginCommandHandler> logger,
    IOptions<JwtSettings> jwtSettings) : IRequestHandler<LoginCommand, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
            return Result<TokenResponse>.Failure(emailResult.Error!, ErrorType.Validation);

        var user = await userRepository.GetUserByEmail(emailResult.Value!);
        if (user is null || !passwordHasher.VerifyPassword(command.Password, user.PasswordHash))
        {
            logger.LogWarning("Invalid credentials for user {Email}", emailResult.Value);
            return Result<TokenResponse>.Failure("Invalid credentials", ErrorType.Validation);
        }
        
        if (passwordHasher.NeedsRehash(user.PasswordHash))
        {
            var hash = passwordHasher.HashPassword(command.Password);
            user.PasswordHash = hash;
            await userRepository.UpdateUser(user);
        }

        await refreshTokenRepository.RevokeAllUserTokens(user.Id, cancellationToken);

        var accessToken = jwtProvider.GenerateToken(user);
        var refreshToken = jwtProvider.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddDays(jwtSettings.Value.RefreshTokenExpirationInDays);

        var refreshTokenResult = Domain.Entities.RefreshToken.Create(user.Id, refreshToken, expiresAt);
        if (refreshTokenResult.IsFailure)
        {
            logger.LogWarning("Failed to create refresh token for user {UserId}", user.Id);
            return Result<TokenResponse>.Failure(refreshTokenResult.Error!, ErrorType.Validation);
        }
        
        await refreshTokenRepository.CreateRefreshToken(refreshTokenResult.Value!, cancellationToken);
        await unitOfWork.CommitAsync();

        return Result<TokenResponse>.Success(new TokenResponse(accessToken, refreshToken, expiresAt));
    }
}