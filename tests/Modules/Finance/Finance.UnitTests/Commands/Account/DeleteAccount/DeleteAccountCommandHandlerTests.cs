using BuildingBlocks.Application.Abstractions;
using Finance.Application.Abstractions.Persistance;
using Finance.Application.Features.Commands.Account.DeleteAccount;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;

namespace Finance.UnitTests.Commands.Account.DeleteAccount;

public sealed class DeleteAccountCommandHandlerTests
{
    private readonly IAccountRepository _accountRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ILogger<DeleteAccountCommandHandler> _loggerMock;
    private readonly DeleteAccountCommandHandler _handler;

    public DeleteAccountCommandHandlerTests()
    {
        _accountRepositoryMock = Substitute.For<IAccountRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<DeleteAccountCommandHandler>>();

        _handler = new DeleteAccountCommandHandler(
            _accountRepositoryMock,
            _unitOfWorkMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAccountIsDeleted()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var command = new DeleteAccountCommand(accountId);
        _accountRepositoryMock.DeleteAccount(accountId).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _accountRepositoryMock.Received(1).DeleteAccount(accountId);
        await _unitOfWorkMock.Received(1).CommitAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAccountNotFound()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var command = new DeleteAccountCommand(accountId);
        _accountRepositoryMock.DeleteAccount(accountId).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Account not found.");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }
}

