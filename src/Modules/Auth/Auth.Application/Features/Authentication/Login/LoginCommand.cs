using Auth.Application.Features.Authentication.Responses;
using MediatR;
using SharedKernel.Common;

namespace Auth.Application.Features.Authentication.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<Result<TokenResponse>>;