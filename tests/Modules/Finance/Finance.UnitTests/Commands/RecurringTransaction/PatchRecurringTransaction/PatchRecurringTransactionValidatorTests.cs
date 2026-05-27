using Finance.Application.Features.Commands.RecurringTransaction.PatchRecurringTransaction;
using FluentValidation.TestHelper;

namespace Finance.UnitTests.Commands.RecurringTransaction.PatchRecurringTransaction;

public sealed class PatchRecurringTransactionValidatorTests
{
    private readonly PatchRecurringTransactionValidator _validator;

    public PatchRecurringTransactionValidatorTests()
    {
        _validator = new PatchRecurringTransactionValidator();
    }

    [Fact]
    public void Should_HaveError_When_IdIsEmpty()
    {
        var command = new PatchRecurringTransactionCommand(Guid.Empty, null, null, null, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_NotHaveError_When_IdIsNotEmpty()
    {
        var command = new PatchRecurringTransactionCommand(Guid.NewGuid(), null, null, null, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

