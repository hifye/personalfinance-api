using MediatR;
using SharedKernel.Common;

namespace Auth.Application.Features.Authentication.UpdatePassword;

public record UpdatePasswordCommand(string CurrentPassword, string NewPassword, string ConfirmNewPassword)
    : IRequest<Result>;