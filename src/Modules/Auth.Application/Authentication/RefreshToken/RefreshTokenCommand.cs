using Auth.Application.Authentication.Responses;
using MediatR;
using SharedKernel.Common;

namespace Auth.Application.Authentication.RefreshToken;

public abstract record RefreshTokenCommand(string RefreshToken) : IRequest<Result<TokenResponse>>;