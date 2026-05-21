using Auth.Application.Abstractions.Authentication;
using Auth.Application.Abstractions.Persistance;
using Auth.Application.Abstractions.Security;
using Auth.Application.Features.Authentication.Responses;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;
using SharedKernel.ValueObjects;

namespace Auth.Application.Features.Authentication.Login;

internal sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider,
    ILogger<LoginCommandHandler> logger,
    IRefreshTokenHasher refreshTokenHasher
) : IRequestHandler<LoginCommand, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(
        LoginCommand command,
        CancellationToken cancellationToken
    )
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
            var newHash = passwordHasher.HashPassword(command.Password);
            var result = user.UpdatePassword(newHash);
            if (result.IsFailure)
                return Result<TokenResponse>.Failure(result.Error!, ErrorType.Validation);

            await userRepository.UpdateUser(user);
        }

        await refreshTokenRepository.RevokeAllUserTokens(user.Id, cancellationToken);

        var tokenResponse = jwtProvider.Generate(user);

        var refreshTokenHash = refreshTokenHasher.Hash(tokenResponse.RefreshToken);

        var refreshTokenResult = Domain.Entities.RefreshToken.Create(
            user.Id,
            refreshTokenHash,
            tokenResponse.RefreshTokenExpiresAt
        );

        if (refreshTokenResult.IsFailure)
        {
            return Result<TokenResponse>.Failure(refreshTokenResult.Error!, ErrorType.Validation);
        }

        await refreshTokenRepository.CreateRefreshToken(
            refreshTokenResult.Value!,
            cancellationToken
        );

        await unitOfWork.CommitAsync();

        return Result<TokenResponse>.Success(tokenResponse);
    }
}
