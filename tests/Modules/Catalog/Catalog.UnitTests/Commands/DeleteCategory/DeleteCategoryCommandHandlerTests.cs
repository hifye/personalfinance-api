using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Abstractions.Persistance;
using Catalog.Application.Features.Commands.DeleteCategory;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;

namespace Catalog.UnitTests.Commands.DeleteCategory;

public sealed class DeleteCategoryCommandHandlerTests
{
    private readonly ICatalogRepository _catalogRepositoryMock;
    private readonly ICurrentUser _currentUserMock;
    private readonly DeleteCategoryCommandHandler _handler;
    private readonly ILogger<DeleteCategoryCommandHandler> _loggerMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public DeleteCategoryCommandHandlerTests()
    {
        _currentUserMock = Substitute.For<ICurrentUser>();
        _catalogRepositoryMock = Substitute.For<ICatalogRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<DeleteCategoryCommandHandler>>();

        _handler = new DeleteCategoryCommandHandler(
            _currentUserMock,
            _catalogRepositoryMock,
            _unitOfWorkMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCategoryIsDeleted()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(categoryId);

        _currentUserMock.UserId.Returns(userId);
        _catalogRepositoryMock.DeleteCategory(categoryId, userId).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _catalogRepositoryMock.Received(1).DeleteCategory(categoryId, userId);
        await _unitOfWorkMock.Received(1).CommitAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCategoryNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(categoryId);

        _currentUserMock.UserId.Returns(userId);
        _catalogRepositoryMock.DeleteCategory(categoryId, userId).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Category not found.");
        result.ErrorType.Should().Be(ErrorType.NotFound);
        await _unitOfWorkMock.DidNotReceive().CommitAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenCategoryBelongsToAnotherUser()
    {
        // Arrange
        var userAId = Guid.NewGuid();
        var userBId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(categoryId);

        _currentUserMock.UserId.Returns(userBId);
        _catalogRepositoryMock.DeleteCategory(categoryId, userBId).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        await _catalogRepositoryMock.Received(1).DeleteCategory(categoryId, userBId);
        await _catalogRepositoryMock.DidNotReceive().DeleteCategory(categoryId, userAId);
        await _unitOfWorkMock.DidNotReceive().CommitAsync();
    }
}