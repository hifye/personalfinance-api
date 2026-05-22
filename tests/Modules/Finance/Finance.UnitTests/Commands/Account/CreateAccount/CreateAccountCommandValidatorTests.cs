using Finance.Application.Features.Commands.Account.CreateAccount;
using Finance.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace Finance.UnitTests.Commands.Account.CreateAccount;

public sealed class CreateAccountCommandValidatorTests
{
    private readonly CreateAccountValidator _validator;

    public CreateAccountCommandValidatorTests()
    {
        _validator = new CreateAccountValidator();
    }

    [Fact]
    public void Should_HaveError_When_NameIsEmpty()
    {
        var command = new CreateAccountCommand("", AccountType.Checking, 100m);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_HaveError_When_NameIsTooLong()
    {
        var command = new CreateAccountCommand(new string('a', 51), AccountType.Checking, 100m);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_HaveError_When_TypeIsNone()
    {
        var command = new CreateAccountCommand("Test", AccountType.None, 100m);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Fact]
    public void Should_NotHaveError_When_CommandIsValid()
    {
        var command = new CreateAccountCommand("Test", AccountType.Checking, 100m);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

