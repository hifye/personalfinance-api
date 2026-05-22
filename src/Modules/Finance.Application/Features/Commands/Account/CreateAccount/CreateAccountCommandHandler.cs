using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Persistance;
using MediatR;
using SharedKernel.Common;

namespace Finance.Application.Features.Commands.Account.CreateAccount;

public class CreateAccountCommandHandler(
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<CreateAccountCommand, Result<Guid>>
{   
    public async Task<Result<Guid>> Handle(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        return await Domain.Entities.Account.Create(currentUser.UserId, command.Name, command.Type, command.InitialBalance)
            .BindAsync(async account =>
            {
                await accountRepository.CreateAccount(account);
                await unitOfWork.CommitAsync();
                return Result<Guid>.Success(account.Id);
            });
    }
}