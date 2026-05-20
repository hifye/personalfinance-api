using MediatR;
using SharedKernel.Common;

namespace Auth.Application.Authentication.Logout;

public abstract record LogoutCommand : IRequest<Result>;