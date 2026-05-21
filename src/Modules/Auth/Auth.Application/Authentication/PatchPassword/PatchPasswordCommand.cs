using MediatR;
using SharedKernel.Common;

namespace Auth.Application.Authentication.PatchPassword;

public record UpdatePasswordCommand(string Password, string ConfirmPassword) : IRequest<Result>;