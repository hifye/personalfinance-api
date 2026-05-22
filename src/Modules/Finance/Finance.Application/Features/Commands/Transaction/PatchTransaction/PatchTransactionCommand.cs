using System.Text.Json.Serialization;
using Finance.Domain.Enums;
using MediatR;
using SharedKernel.Common;

namespace Finance.Application.Features.Commands.Transaction.PatchTransaction;

public record PatchTransactionCommand([property: JsonIgnore] Guid Id, TransactionType? Type, string? Description) : IRequest<Result>;