using Finance.Domain.Enums;
using MediatR;
using SharedKernel.Common;

namespace Finance.Application.Features.Commands.Account.CreateAccount;

public record CreateAccountCommand(string Name, AccountType Type, decimal InitialBalance) : IRequest<Result<Guid>>;