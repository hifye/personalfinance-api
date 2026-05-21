using MediatR;
using SharedKernel.Common;

namespace Auth.Application.Features.Authentication.PatchPassword;

public record UpdatePasswordCommand(string Password, string ConfirmPassword) : IRequest<Result>;