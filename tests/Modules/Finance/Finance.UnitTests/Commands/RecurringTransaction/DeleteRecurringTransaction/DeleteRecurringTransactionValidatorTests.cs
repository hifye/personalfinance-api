using Finance.Application.Features.Commands.RecurringTransaction.DeleteRecurringTransaction;
using FluentValidation.TestHelper;
using Xunit;

namespace Finance.UnitTests.Commands.RecurringTransaction.DeleteRecurringTransaction;

public sealed class DeleteRecurringTransactionValidatorTests
{
    private readonly DeleteRecurringTransactionValidator _validator;

    public DeleteRecurringTransactionValidatorTests()
    {
        _validator = new DeleteRecurringTransactionValidator();
    }

    [Fact]
    public void Should_HaveError_When_IdIsEmpty()
    {
        var command = new DeleteRecurringTransactionCommand(Guid.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_NotHaveError_When_IdIsNotEmpty()
    {
        var command = new DeleteRecurringTransactionCommand(Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

