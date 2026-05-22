using Finance.Application.Features.Commands.RecurringTransaction.CreateRecurringTransaction;
using Finance.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace Finance.UnitTests.Commands.RecurringTransaction.CreateRecurringTransaction;

public sealed class CreateRecurringTransactionValidatorTests
{
    private readonly CreateRecurringTransactionValidator _validator;

    public CreateRecurringTransactionValidatorTests()
    {
        _validator = new CreateRecurringTransactionValidator();
    }

    [Fact]
    public void Should_HaveError_When_AccountIdIsEmpty()
    {
        var command = new CreateRecurringTransactionCommand(Guid.Empty, Guid.NewGuid(), 100m, TransactionType.Expense, "Description", RecurringFrequency.Monthly);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.AccountId);
    }

    [Fact]
    public void Should_HaveError_When_AmountIsZeroOrNegative()
    {
        var command = new CreateRecurringTransactionCommand(Guid.NewGuid(), Guid.NewGuid(), 0m, TransactionType.Expense, "Description", RecurringFrequency.Monthly);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Should_NotHaveError_When_CommandIsValid()
    {
        var command = new CreateRecurringTransactionCommand(Guid.NewGuid(), Guid.NewGuid(), 100m, TransactionType.Expense, "Description", RecurringFrequency.Monthly);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

