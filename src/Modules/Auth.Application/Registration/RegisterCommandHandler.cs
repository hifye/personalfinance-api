using Auth.Application.Abstractions.Authentication;
using Auth.Application.Abstractions.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;
using SharedKernel.ValueObjects;

namespace Auth.Application.Registration;

internal sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ILogger<RegisterCommandHandler> logger) : IRequestHandler<RegisterCommand, Result>
{
    public async Task<Result> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var hashedPassword = passwordHasher.HashPassword(command.Password);

        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
        {
            logger.LogWarning("Invalid email {Email}", command.Email);
            return Result.Failure(emailResult.Error!, ErrorType.Validation);
        }
        
        var emailExists = await userRepository.GetUserByEmail(emailResult.Value!);
        if (emailExists is not null)
        {
            logger.LogWarning("Email {Email} already exists", emailResult.Value);
            return Result.Failure("Email already exists", ErrorType.Conflict);
        }

        var register = Domain.Entities.User.Create(command.Name, command.Email, hashedPassword);
        if (register.IsFailure)
        {
            logger.LogWarning("Failed to create user {Name}", command.Name);
            return Result.Failure(register.Error!, ErrorType.Validation);
        }
        
        await userRepository.CreateUser(register.Value!);
        await unitOfWork.CommitAsync();

        return Result.Success();
    }
}