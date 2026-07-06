using Auth.Application.Abstractions.Authentication;
using Auth.Application.Abstractions.Persistance;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Auth.Application.Features.Authentication.UpdatePassword;

public sealed class UpdatePasswordCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ICurrentUser currentUser,
    IRefreshTokenRepository refreshTokenRepository,
    ILogger<UpdatePasswordCommandHandler> logger)
    : IRequestHandler<UpdatePasswordCommand, Result>
{
    public async Task<Result> Handle(UpdatePasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserById(currentUser.UserId);
        if (user is null)
        {
            logger.LogWarning("User not found for id {UserId}", currentUser.UserId);
            return Result.Failure("User not found", ErrorType.NotFound);
        }

        if (!passwordHasher.VerifyPassword(user.PasswordHash, command.CurrentPassword))
        {
            logger.LogWarning("Current password is incorrect for user {UserId}", currentUser.UserId);
            return Result.Failure("Current password is incorrect", ErrorType.Validation);
        }

        var hash = passwordHasher.HashPassword(command.NewPassword);

        var result = user.UpdatePassword(hash);
        if (result.IsFailure)
        {
            logger.LogWarning("Failed to update password for user {UserId}", currentUser.UserId);
            return Result.Failure("Failed to update password", ErrorType.Validation);
        }

        await userRepository.UpdateUser(user);
        await unitOfWork.CommitAsync();

        await refreshTokenRepository.RevokeAllUserTokens(currentUser.UserId, cancellationToken);
        return Result.Success();
    }
}