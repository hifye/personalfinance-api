using System.Text.Json.Serialization;
using Finance.Domain.Enums;
using MediatR;
using SharedKernel.Common;

namespace Finance.Application.Features.Commands.Account.PatchAccount;

public record PatchAccountCommand([property: JsonIgnore] Guid Id, AccountType? Type, bool? IsActive) : IRequest<Result>;