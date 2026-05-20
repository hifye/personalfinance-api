using MediatR;
using SharedKernel.Common;

namespace Auth.Application.Authentication.Logout;

public sealed record LogoutCommand : IRequest<Result>;