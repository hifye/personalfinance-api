using Auth.Application.Abstractions.Authentication;
using Auth.Application.Abstractions.Persistance;
using Auth.Application.Abstractions.Security;
using Auth.Application.Authentication.Responses;
using MediatR;
using SharedKernel.Common;

namespace Auth.Application.Authentication.RefreshToken;

internal sealed class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IJwtProvider jwtProvider,
    IRefreshTokenHasher refreshTokenHasher
) : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken
    )
    {
        var tokenHash = refreshTokenHasher.Hash(command.RefreshToken);

        var existing = await refreshTokenRepository.GetRefreshTokenByHash(
            tokenHash,
            cancellationToken
        );
        if (existing is null)
            return Result<TokenResponse>.Failure(
                "Invalid or expired refresh token.",
                ErrorType.Validation
            );

        var user = await userRepository.GetUserById(existing.UserId);
        if (user is null)
            return Result<TokenResponse>.Failure("User not found.", ErrorType.NotFound);

        var revokeResult = existing.Revoke();
        if (revokeResult.IsFailure)
            return Result<TokenResponse>.Failure(revokeResult.Error!, ErrorType.Validation);

        await refreshTokenRepository.RevokeRefreshToken(existing.Id, cancellationToken);

        var tokenResponse = jwtProvider.Generate(user);
        var newRefreshTokenHash = refreshTokenHasher.Hash(tokenResponse.RefreshToken);

        var refreshTokenResult = Domain.Entities.RefreshToken.Create(
            user.Id,
            newRefreshTokenHash,
            tokenResponse.RefreshTokenExpiresAt
        );
        
        if (refreshTokenResult.IsFailure)
            return Result<TokenResponse>.Failure(refreshTokenResult.Error!, ErrorType.Validation);

        await refreshTokenRepository.CreateRefreshToken(
            refreshTokenResult.Value!,
            cancellationToken
        );
        await unitOfWork.CommitAsync();

        return Result<TokenResponse>.Success(tokenResponse);
    }
}
