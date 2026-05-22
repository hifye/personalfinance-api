using Finance.Application.Features.Commands.Account.DeleteAccount;
using FluentValidation.TestHelper;
using Xunit;

namespace Finance.UnitTests.Commands.Account.DeleteAccount;

public class DeleteAccountCommandValidatorTests
{
    private readonly DeleteAccountCommandValidator _validator;

    public DeleteAccountCommandValidatorTests()
    {
        _validator = new DeleteAccountCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_IdIsEmpty()
    {
        var command = new DeleteAccountCommand(Guid.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_NotHaveError_When_IdIsNotEmpty()
    {
        var command = new DeleteAccountCommand(Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
