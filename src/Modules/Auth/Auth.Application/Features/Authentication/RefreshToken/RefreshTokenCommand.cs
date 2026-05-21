using Auth.Application.Features.Authentication.Responses;
using MediatR;
using SharedKernel.Common;

namespace Auth.Application.Features.Authentication.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<TokenResponse>>;