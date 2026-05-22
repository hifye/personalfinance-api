using Finance.Application.Features.Commands.Transaction.CreateTransaction;
using Finance.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace Finance.UnitTests.Commands.Transaction.CreateTransaction;

public sealed class CreateTransactionValidatorTests
{
    private readonly CreateTransactionValidator _validator;

    public CreateTransactionValidatorTests()
    {
        _validator = new CreateTransactionValidator();
    }

    [Fact]
    public void Should_HaveError_When_AccountIdIsEmpty()
    {
        var command = new CreateTransactionCommand(Guid.Empty, Guid.NewGuid(), null, 100m, TransactionType.Expense, "Description");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.AccountId);
    }

    [Fact]
    public void Should_HaveError_When_CategoryIdIsEmpty()
    {
        var command = new CreateTransactionCommand(Guid.NewGuid(), Guid.Empty, null, 100m, TransactionType.Expense, "Description");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void Should_HaveError_When_AmountIsZeroOrNegative()
    {
        var command = new CreateTransactionCommand(Guid.NewGuid(), Guid.NewGuid(), null, 0m, TransactionType.Expense, "Description");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Should_HaveError_When_DescriptionIsTooLong()
    {
        var command = new CreateTransactionCommand(Guid.NewGuid(), Guid.NewGuid(), null, 100m, TransactionType.Expense, new string('a', 251));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_NotHaveError_When_CommandIsValid()
    {
        var command = new CreateTransactionCommand(Guid.NewGuid(), Guid.NewGuid(), null, 100m, TransactionType.Expense, "Description");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

