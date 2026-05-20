using MediatR;
using SharedKernel.Common;

namespace Auth.Application.Registration;

public abstract record RegisterCommand(
    string Name,
    string Email,
    string Password,
    string ConfirmPassword) : IRequest<Result>;