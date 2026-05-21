using Auth.Application.Authentication.Responses;
using MediatR;
using SharedKernel.Common;

namespace Auth.Application.Authentication.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<TokenResponse>>;