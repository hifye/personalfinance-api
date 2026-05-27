using Finance.Application.Features.Commands.Transaction.DeleteTransaction;
using FluentValidation.TestHelper;

namespace Finance.UnitTests.Commands.Transaction.DeleteTransaction;

public sealed class DeleteTransactionCommandValidatorTests
{
    private readonly DeleteTransactionCommandValidator _validator;

    public DeleteTransactionCommandValidatorTests()
    {
        _validator = new DeleteTransactionCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_IdIsEmpty()
    {
        var command = new DeleteTransactionCommand(Guid.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_NotHaveError_When_IdIsNotEmpty()
    {
        var command = new DeleteTransactionCommand(Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

