using Auth.Application.Abstractions.Authentication;
using Auth.Application.Abstractions.Persistance;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using SharedKernel.Common;

namespace Auth.Application.Authentication.Logout;

internal sealed class LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork, ICurrentUser currentUser)
    : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        await refreshTokenRepository.RevokeAllUserTokens(currentUser.UserId, cancellationToken);
        await unitOfWork.CommitAsync();
        return Result.Success();
    }
}