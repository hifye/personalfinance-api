using MediatR;
using SharedKernel.Common;

namespace Auth.Application.Registration;

public sealed record RegisterCommand(
    string Name,
    string Email,
    string Password,
    string ConfirmPassword) : IRequest<Result>;