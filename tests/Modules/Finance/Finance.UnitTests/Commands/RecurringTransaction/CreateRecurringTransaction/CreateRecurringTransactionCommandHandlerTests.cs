using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Contracts;
using Finance.Application.Abstractions.Persistance;
using Finance.Application.Features.Commands.RecurringTransaction.CreateRecurringTransaction;
using Finance.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;

namespace Finance.UnitTests.Commands.RecurringTransaction.CreateRecurringTransaction;

public sealed class CreateRecurringTransactionCommandHandlerTests
{
    private readonly IAccountRepository _accountRepositoryMock;
    private readonly ICatalogModule _catalogModuleMock;
    private readonly ICurrentUser _currentUserMock;
    private readonly CreateRecurringTransactionCommandHandler _handler;
    private readonly ILogger<CreateRecurringTransactionCommandHandler> _loggerMock;
    private readonly IRecurringTransactionRepository _recurringTransactionRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public CreateRecurringTransactionCommandHandlerTests()
    {
        _recurringTransactionRepositoryMock = Substitute.For<IRecurringTransactionRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _catalogModuleMock = Substitute.For<ICatalogModule>();
        _accountRepositoryMock = Substitute.For<IAccountRepository>();
        _currentUserMock = Substitute.For<ICurrentUser>();
        _loggerMock = Substitute.For<ILogger<CreateRecurringTransactionCommandHandler>>();

        _handler = new CreateRecurringTransactionCommandHandler(
            _recurringTransactionRepositoryMock,
            _unitOfWorkMock,
            _catalogModuleMock,
            _accountRepositoryMock,
            _currentUserMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenRecurringTransactionIsCreated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var command = new CreateRecurringTransactionCommand(accountId, categoryId, 100m, TransactionType.Expense,
            "Description", RecurringFrequency.Monthly);

        _currentUserMock.UserId.Returns(userId);
        _accountRepositoryMock.GetAccountById(accountId, userId).Returns(Finance.Domain.Entities.Account
            .Create(userId, "Account", AccountType.Checking, 1000m).Value);
        _catalogModuleMock.CategoryExistsAsync(categoryId, userId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        await _recurringTransactionRepositoryMock.Received(1)
            .CreateRecurringTransaction(Arg.Any<Finance.Domain.Entities.RecurringTransaction>());
        await _unitOfWorkMock.Received(1).CommitAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAccountNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateRecurringTransactionCommand(Guid.NewGuid(), Guid.NewGuid(), 100m,
            TransactionType.Expense, "Description", RecurringFrequency.Monthly);

        _currentUserMock.UserId.Returns(userId);
        _accountRepositoryMock.GetAccountById(command.AccountId, userId)
            .Returns((Finance.Domain.Entities.Account?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Account not found");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCategoryNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateRecurringTransactionCommand(Guid.NewGuid(), Guid.NewGuid(), 100m,
            TransactionType.Expense, "Description", RecurringFrequency.Monthly);

        _currentUserMock.UserId.Returns(userId);
        _accountRepositoryMock.GetAccountById(command.AccountId, userId).Returns(Finance.Domain.Entities.Account
            .Create(userId, "Account", AccountType.Checking, 1000m).Value);
        _catalogModuleMock.CategoryExistsAsync(command.CategoryId, userId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Category not found");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenAccountBelongsToAnotherUser()
    {
        // Arrange
        var userAId = Guid.NewGuid();
        var userBId = Guid.NewGuid();
        var command = new CreateRecurringTransactionCommand(Guid.NewGuid(), Guid.NewGuid(), 100m,
            TransactionType.Expense, "Description", RecurringFrequency.Monthly);

        _currentUserMock.UserId.Returns(userBId);
        _accountRepositoryMock.GetAccountById(command.AccountId, userBId)
            .Returns((Finance.Domain.Entities.Account?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        await _accountRepositoryMock.Received(1).GetAccountById(command.AccountId, userBId);
        await _accountRepositoryMock.DidNotReceive().GetAccountById(command.AccountId, userAId);
        await _catalogModuleMock.DidNotReceive()
            .CategoryExistsAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _recurringTransactionRepositoryMock.DidNotReceive()
            .CreateRecurringTransaction(Arg.Any<Finance.Domain.Entities.RecurringTransaction>());
        await _unitOfWorkMock.DidNotReceive().CommitAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenCategoryBelongsToAnotherUser()
    {
        // Arrange
        var userAId = Guid.NewGuid();
        var userBId = Guid.NewGuid();
        var command = new CreateRecurringTransactionCommand(Guid.NewGuid(), Guid.NewGuid(), 100m,
            TransactionType.Expense, "Description", RecurringFrequency.Monthly);

        _currentUserMock.UserId.Returns(userBId);
        _accountRepositoryMock.GetAccountById(command.AccountId, userBId).Returns(Finance.Domain.Entities.Account
            .Create(userBId, "Account", AccountType.Checking, 1000m).Value);
        _catalogModuleMock.CategoryExistsAsync(command.CategoryId, userBId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        await _catalogModuleMock.Received(1)
            .CategoryExistsAsync(command.CategoryId, userBId, Arg.Any<CancellationToken>());
        await _catalogModuleMock.DidNotReceive()
            .CategoryExistsAsync(command.CategoryId, userAId, Arg.Any<CancellationToken>());
        await _recurringTransactionRepositoryMock.DidNotReceive()
            .CreateRecurringTransaction(Arg.Any<Finance.Domain.Entities.RecurringTransaction>());
        await _unitOfWorkMock.DidNotReceive().CommitAsync();
    }
}