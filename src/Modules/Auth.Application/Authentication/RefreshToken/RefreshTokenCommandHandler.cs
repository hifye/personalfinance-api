using Auth.Application.Abstractions.Authentication;
using Auth.Application.Abstractions.Persistance;
using Auth.Application.Authentication.Responses;
using Auth.Application.Settings;
using MediatR;
using Microsoft.Extensions.Options;
using SharedKernel.Common;

namespace Auth.Application.Authentication.RefreshToken;

internal sealed class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IJwtProvider jwtProvider,
    IOptions<JwtSettings> jwtSettings)
    : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var existing  = await refreshTokenRepository.GetRefreshToken(command.RefreshToken, cancellationToken);
        if(existing is null)
            return Result<TokenResponse>.Failure("Invalid or expired refresh token.", ErrorType.Validation);
        
        var user = await userRepository.GetUserById(existing.UserId);
        if(user is null)
            return Result<TokenResponse>.Failure("User not found.", ErrorType.NotFound);

        var revokeResult = existing.Revoke();
        if(revokeResult.IsFailure)
            return Result<TokenResponse>.Failure(revokeResult.Error!, ErrorType.Validation);

        await refreshTokenRepository.RevokeRefreshToken(existing.Id, cancellationToken);
        
        var accessToken = jwtProvider.GenerateToken(user);
        var rawRefreshToken = jwtProvider.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddDays(jwtSettings.Value.RefreshTokenExpirationInDays);
        
        var newRefreshToken = Domain.Entities.RefreshToken.Create(user.Id, rawRefreshToken, expiresAt);
        if (newRefreshToken.IsFailure)
            return Result<TokenResponse>.Failure(newRefreshToken.Error!, ErrorType.Validation);
        
        await refreshTokenRepository.CreateRefreshToken(newRefreshToken.Value!, cancellationToken);
        await unitOfWork.CommitAsync();
        
        return Result<TokenResponse>.Success(new TokenResponse(accessToken, rawRefreshToken, expiresAt));
    }
}