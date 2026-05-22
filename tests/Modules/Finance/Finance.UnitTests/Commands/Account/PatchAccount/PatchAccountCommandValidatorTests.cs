using Finance.Application.Features.Commands.Account.PatchAccount;
using Finance.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace Finance.UnitTests.Commands.Account.PatchAccount;

public class PatchAccountCommandValidatorTests
{
    private readonly PatchAccountCommandValidator _validator;

    public PatchAccountCommandValidatorTests()
    {
        _validator = new PatchAccountCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_IdIsEmpty()
    {
        var command = new PatchAccountCommand(Guid.Empty, AccountType.Checking, true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_HaveError_When_TypeIsInvalid()
    {
        var command = new PatchAccountCommand(Guid.NewGuid(), (AccountType)999, true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Fact]
    public void Should_NotHaveError_When_CommandIsValid()
    {
        var command = new PatchAccountCommand(Guid.NewGuid(), AccountType.Checking, true);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
