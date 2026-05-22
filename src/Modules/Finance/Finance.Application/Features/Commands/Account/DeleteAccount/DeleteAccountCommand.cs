using MediatR;
using SharedKernel.Common;

namespace Finance.Application.Features.Commands.Account.DeleteAccount;

public record DeleteAccountCommand(Guid Id) : IRequest<Result>;