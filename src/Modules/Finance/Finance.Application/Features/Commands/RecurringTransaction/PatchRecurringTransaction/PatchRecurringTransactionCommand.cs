using System.Text.Json.Serialization;
using Finance.Domain.Enums;
using MediatR;
using SharedKernel.Common;

namespace Finance.Application.Features.Commands.RecurringTransaction.PatchRecurringTransaction;

public record PatchRecurringTransactionCommand(
    [property: JsonIgnore] Guid Id,
    decimal? Amount,
    TransactionType? Type,
    string? Description,
    RecurringFrequency? Frequency,
    bool? IsActive
) : IRequest<Result>;