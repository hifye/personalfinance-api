using FluentValidation;

namespace Finance.Application.Features.Commands.RecurringTransaction.DeleteRecurringTransaction;

public sealed class DeleteRecurringTransactionValidator : AbstractValidator<DeleteRecurringTransactionCommand>
{
    public DeleteRecurringTransactionValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");
    }
}
