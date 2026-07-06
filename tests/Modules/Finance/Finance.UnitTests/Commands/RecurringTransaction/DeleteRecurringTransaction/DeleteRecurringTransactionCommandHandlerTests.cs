using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Persistance;
using Finance.Application.Features.Commands.RecurringTransaction.DeleteRecurringTransaction;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;

namespace Finance.UnitTests.Commands.RecurringTransaction.DeleteRecurringTransaction;

public sealed class DeleteRecurringTransactionCommandHandlerTests
{
    private readonly ICurrentUser _currentUserMock;
    private readonly DeleteRecurringTransactionCommandHandler _handler;
    private readonly ILogger<DeleteRecurringTransactionCommandHandler> _loggerMock;
    private readonly IRecurringTransactionRepository _recurringTransactionRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public DeleteRecurringTransactionCommandHandlerTests()
    {
        _currentUserMock = Substitute.For<ICurrentUser>();
        _recurringTransactionRepositoryMock = Substitute.For<IRecurringTransactionRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<DeleteRecurringTransactionCommandHandler>>();

        _handler = new DeleteRecurringTransactionCommandHandler(
            _recurringTransactionRepositoryMock,
            _unitOfWorkMock,
            _currentUserMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenRecurringTransactionIsDeleted()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var command = new DeleteRecurringTransactionCommand(id);

        _currentUserMock.UserId.Returns(userId);
        _recurringTransactionRepositoryMock.DeleteRecurringTransaction(id, userId).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _recurringTransactionRepositoryMock.Received(1).DeleteRecurringTransaction(id, userId);
        await _unitOfWorkMock.Received(1).CommitAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRecurringTransactionNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var command = new DeleteRecurringTransactionCommand(id);

        _currentUserMock.UserId.Returns(userId);
        _recurringTransactionRepositoryMock.DeleteRecurringTransaction(id, userId).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Recurring Transaction not found.");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenRecurringTransactionBelongsToAnotherUser()
    {
        // Arrange
        var userAId = Guid.NewGuid();
        var userBId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var command = new DeleteRecurringTransactionCommand(id);

        _currentUserMock.UserId.Returns(userBId);
        _recurringTransactionRepositoryMock.DeleteRecurringTransaction(id, userBId).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        await _recurringTransactionRepositoryMock.Received(1).DeleteRecurringTransaction(id, userBId);
        await _recurringTransactionRepositoryMock.DidNotReceive().DeleteRecurringTransaction(id, userAId);
        await _unitOfWorkMock.DidNotReceive().CommitAsync();
    }
}