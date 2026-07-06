using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Contracts;
using Finance.Application.Abstractions.Persistance;
using Finance.Application.Features.Commands.Transaction.CreateTransaction;
using Finance.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;

namespace Finance.UnitTests.Commands.Transaction.CreateTransaction;

public sealed class CreateTransactionCommandHandlerTests
{
    private readonly IAccountRepository _accountRepositoryMock;
    private readonly ICatalogModule _catalogModuleMock;
    private readonly ICurrentUser _currentUserMock;
    private readonly CreateTransactionCommandHandler _handler;
    private readonly ILogger<CreateTransactionCommandHandler> _loggerMock;
    private readonly IRecurringTransactionRepository _recurringTransactionRepositoryMock;
    private readonly ITransactionRepository _transactionRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public CreateTransactionCommandHandlerTests()
    {
        _recurringTransactionRepositoryMock = Substitute.For<IRecurringTransactionRepository>();
        _transactionRepositoryMock = Substitute.For<ITransactionRepository>();
        _catalogModuleMock = Substitute.For<ICatalogModule>();
        _accountRepositoryMock = Substitute.For<IAccountRepository>();
        _currentUserMock = Substitute.For<ICurrentUser>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<CreateTransactionCommandHandler>>();

        _handler = new CreateTransactionCommandHandler(
            _recurringTransactionRepositoryMock,
            _transactionRepositoryMock,
            _catalogModuleMock,
            _accountRepositoryMock,
            _currentUserMock,
            _unitOfWorkMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTransactionIsCreated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var command = new CreateTransactionCommand(accountId, categoryId, null, 100m, TransactionType.Expense,
            "Test Transaction");

        _currentUserMock.UserId.Returns(userId);
        _catalogModuleMock.CategoryExistsAsync(categoryId, userId, Arg.Any<CancellationToken>()).Returns(true);
        _accountRepositoryMock.GetAccountById(accountId, userId).Returns(Finance.Domain.Entities.Account
            .Create(userId, "Account", AccountType.Checking, 1000m).Value);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        await _transactionRepositoryMock.Received(1).CreateTransaction(Arg.Any<Finance.Domain.Entities.Transaction>());
        await _unitOfWorkMock.Received(1).CommitAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCategoryNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateTransactionCommand(Guid.NewGuid(), Guid.NewGuid(), null, 100m, TransactionType.Expense,
            "Test Transaction");

        _currentUserMock.UserId.Returns(userId);
        _catalogModuleMock.CategoryExistsAsync(command.CategoryId, userId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Category not found.");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAccountNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateTransactionCommand(Guid.NewGuid(), Guid.NewGuid(), null, 100m, TransactionType.Expense,
            "Test Transaction");

        _currentUserMock.UserId.Returns(userId);
        _catalogModuleMock.CategoryExistsAsync(command.CategoryId, userId, Arg.Any<CancellationToken>()).Returns(true);
        _accountRepositoryMock.GetAccountById(command.AccountId, userId)
            .Returns((Finance.Domain.Entities.Account?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Account not found.");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenAccountBelongsToAnotherUser()
    {
        // Arrange
        var userAId = Guid.NewGuid();
        var userBId = Guid.NewGuid();
        var command = new CreateTransactionCommand(Guid.NewGuid(), Guid.NewGuid(), null, 100m, TransactionType.Expense,
            "Test Transaction");

        _currentUserMock.UserId.Returns(userBId);
        _catalogModuleMock.CategoryExistsAsync(command.CategoryId, userBId, Arg.Any<CancellationToken>()).Returns(true);
        _accountRepositoryMock.GetAccountById(command.AccountId, userBId)
            .Returns((Finance.Domain.Entities.Account?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        await _accountRepositoryMock.Received(1).GetAccountById(command.AccountId, userBId);
        await _accountRepositoryMock.DidNotReceive().GetAccountById(command.AccountId, userAId);
        await _transactionRepositoryMock.DidNotReceive()
            .CreateTransaction(Arg.Any<Finance.Domain.Entities.Transaction>());
        await _unitOfWorkMock.DidNotReceive().CommitAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenCategoryBelongsToAnotherUser()
    {
        // Arrange
        var userAId = Guid.NewGuid();
        var userBId = Guid.NewGuid();
        var command = new CreateTransactionCommand(Guid.NewGuid(), Guid.NewGuid(), null, 100m, TransactionType.Expense,
            "Test Transaction");

        _currentUserMock.UserId.Returns(userBId);
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
        await _accountRepositoryMock.DidNotReceive().GetAccountById(Arg.Any<Guid>(), Arg.Any<Guid>());
        await _transactionRepositoryMock.DidNotReceive()
            .CreateTransaction(Arg.Any<Finance.Domain.Entities.Transaction>());
    }
}