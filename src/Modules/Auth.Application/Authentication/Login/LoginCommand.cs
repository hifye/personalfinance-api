using Auth.Application.Authentication.Responses;
using MediatR;
using SharedKernel.Common;

namespace Auth.Application.Authentication.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<Result<TokenResponse>>;