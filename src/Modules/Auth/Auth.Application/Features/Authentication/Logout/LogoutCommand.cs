using MediatR;
using SharedKernel.Common;

namespace Auth.Application.Features.Authentication.Logout;

public sealed record LogoutCommand : IRequest<Result>;