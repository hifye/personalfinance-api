using FluentValidation;

namespace Auth.Application.Features.Authentication.UpdatePassword;

public sealed class UpdatePasswordValidator : AbstractValidator<UpdatePasswordCommand>
{
    public UpdatePasswordValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(12).WithMessage("New password must be at least 12 characters long.")
            .Matches("[A-Z]").WithMessage("New password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("New password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("New password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().WithMessage("Confirm New Password is required.")
            .Equal(x => x.NewPassword).WithMessage("New passwords do not match.");
    }
}