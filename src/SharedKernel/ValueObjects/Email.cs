using System.Net.Mail;
using SharedKernel.Common;

namespace SharedKernel.ValueObjects;

public sealed record Email
{
    public string Address { get; } = null!;

    private Email(string address) => Address = address;

    public static Result<Email> Create(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return Result<Email>.Failure("Email cannot be empty", ErrorType.Validation);

        address = address.Trim().ToLower();

        return Guard.AgainstOutOfRange(address.Length > 100,
                "Email cannot be longer than 100 characters."
            )
            .Bind(() => Result.Try(() => new MailAddress(address), "Invalid Email"))
            .Map(() => new Email(address));
    }
}