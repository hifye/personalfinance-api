using Finance.Application.Features.Commands.Transaction.PatchTransaction;
using Finance.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace Finance.UnitTests.Commands.Transaction.PatchTransaction;

public class PatchTransactionCommandValidatorTests
{
    private readonly PatchTransactionCommandValidator _validator;

    public PatchTransactionCommandValidatorTests()
    {
        _validator = new PatchTransactionCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_IdIsEmpty()
    {
        var command = new PatchTransactionCommand(Guid.Empty, TransactionType.Expense, "Description");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_HaveError_When_DescriptionIsTooLong()
    {
        var command = new PatchTransactionCommand(Guid.NewGuid(), TransactionType.Expense, new string('a', 251));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_NotHaveError_When_CommandIsValid()
    {
        var command = new PatchTransactionCommand(Guid.NewGuid(), TransactionType.Expense, "Description");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
