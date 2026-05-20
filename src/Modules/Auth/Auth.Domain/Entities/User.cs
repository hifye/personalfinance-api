using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities.Auth;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private User(string name, Email email, string passwordHash)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
    }
    
    public static Result<User> Create(string name, string email, string passwordHash)
    {
        return Guard
            .AgainstNullOrWhiteSpace(name, "Name cannot be empty.")
            .Bind(() => name.Length > 200
                ? Result.Failure("Name cannot be longer than 200 characters.", ErrorType.Validation)
                : Result.Success())
            .Bind(() => Guard.AgainstNullOrWhiteSpace(passwordHash, "Password cannot be empty."))
            .Bind(() => Email.Create(email))
            .Map(validEmail => new User(name, validEmail, passwordHash));
    }

    public Result UpdatePassword(string passwordHash)
    {
        return Guard.AgainstNullOrWhiteSpace(passwordHash, "Password cannot be empty.")
            .Bind(() =>
            {
                PasswordHash = passwordHash;
                UpdatedAt = DateTime.UtcNow;
                return Result.Success();
            });
    }
    
    protected User() { }
}