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
    private readonly ICurrentUser _currentUserMock;
    private readonly PatchAccountCommandHandler _handler;
    private readonly ILogger<PatchAccountCommandHandler> _loggerMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public PatchAccountCommandHandlerTests()
    {
        _currentUserMock = Substitute.For<ICurrentUser>();
        _accountRepositoryMock = Substitute.For<IAccountRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<PatchAccountCommandHandler>>();

        _handler = new PatchAccountCommandHandler(
            _currentUserMock,
            _accountRepositoryMock,
            _unitOfWorkMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAccountIsPatched()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var account = Finance.Domain.Entities.Account.Create(userId, "Old Name", AccountType.Checking, 100m).Value;
        var command = new PatchAccountCommand(accountId, AccountType.Savings, false);

        _currentUserMock.UserId.Returns(userId);
        _accountRepositoryMock.GetAccountById(accountId, userId).Returns(account);

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
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var command = new PatchAccountCommand(accountId, AccountType.Savings, false);

        _currentUserMock.UserId.Returns(userId);
        _accountRepositoryMock.GetAccountById(accountId, userId).Returns((Finance.Domain.Entities.Account?)null);

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
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var account = Finance.Domain.Entities.Account.Create(userId, "Old Name", AccountType.Checking, 100m).Value;
        // Ambos nulos para forcar falha na regra de dominio do Patch (pelo menos um campo deve ser fornecido)
        var command = new PatchAccountCommand(accountId, null, null);

        _currentUserMock.UserId.Returns(userId);
        _accountRepositoryMock.GetAccountById(accountId, userId).Returns(account);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("At least one field must be provided for patching.");
        await _unitOfWorkMock.DidNotReceive().CommitAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenAccountBelongsToAnotherUser()
    {
        // Arrange
        var userAId = Guid.NewGuid();
        var userBId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var command = new PatchAccountCommand(accountId, AccountType.Savings, false);

        _currentUserMock.UserId.Returns(userBId);
        _accountRepositoryMock.GetAccountById(accountId, userBId).Returns((Finance.Domain.Entities.Account?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        await _accountRepositoryMock.Received(1).GetAccountById(accountId, userBId);
        await _accountRepositoryMock.DidNotReceive().GetAccountById(accountId, userAId);
        await _accountRepositoryMock.DidNotReceive().UpdateAccount(Arg.Any<Finance.Domain.Entities.Account>());
        await _unitOfWorkMock.DidNotReceive().CommitAsync();
    }
}