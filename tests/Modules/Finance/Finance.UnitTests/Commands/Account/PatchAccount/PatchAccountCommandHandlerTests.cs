using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Persistance;
using Finance.Application.Features.Commands.Account.PatchAccount;
using Finance.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;

namespace Finance.UnitTests.Commands.Account.PatchAccount;

public sealed class PatchAccountCommandHandlerTests
{
    private readonly IAccountRepository _accountRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ILogger<PatchAccountCommandHandler> _loggerMock;
    private readonly PatchAccountCommandHandler _handler;

    public PatchAccountCommandHandlerTests()
    {
        _accountRepositoryMock = Substitute.For<IAccountRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<PatchAccountCommandHandler>>();

        _handler = new PatchAccountCommandHandler(
            _accountRepositoryMock,
            _unitOfWorkMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAccountIsPatched()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = Finance.Domain.Entities.Account.Create(Guid.NewGuid(), "Old Name", AccountType.Checking, 100m).Value;
        var command = new PatchAccountCommand(accountId, AccountType.Savings, false);

        _accountRepositoryMock.GetAccountById(accountId).Returns(account);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        account!.Type.Should().Be(AccountType.Savings);
        account.IsActive.Should().BeFalse();
        await _accountRepositoryMock.Received(1).UpdateAccount(account);
        await _unitOfWorkMock.Received(1).CommitAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAccountNotFound()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var command = new PatchAccountCommand(accountId, AccountType.Savings, false);

        _accountRepositoryMock.GetAccountById(accountId).Returns((Finance.Domain.Entities.Account?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Account not found.");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPatchValidationFails()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = Finance.Domain.Entities.Account.Create(Guid.NewGuid(), "Old Name", AccountType.Checking, 100m).Value;
        // Ambos nulos para for�ar falha na regra de dom�nio do Patch (pelo menos um campo deve ser fornecido)
        var command = new PatchAccountCommand(accountId, null, null);

        _accountRepositoryMock.GetAccountById(accountId).Returns(account);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("At least one field must be provided for patching.");
        await _unitOfWorkMock.DidNotReceive().CommitAsync();
    }
}

