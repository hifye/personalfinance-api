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
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ILogger<DeleteCategoryCommandHandler> _loggerMock;
    private readonly DeleteCategoryCommandHandler _handler;

    public DeleteCategoryCommandHandlerTests()
    {
        _catalogRepositoryMock = Substitute.For<ICatalogRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<DeleteCategoryCommandHandler>>();

        _handler = new DeleteCategoryCommandHandler(
            _catalogRepositoryMock,
            _unitOfWorkMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCategoryIsDeleted()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(categoryId);
        _catalogRepositoryMock.DeleteCategory(categoryId).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _catalogRepositoryMock.Received(1).DeleteCategory(categoryId);
        await _unitOfWorkMock.Received(1).CommitAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCategoryNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(categoryId);
        _catalogRepositoryMock.DeleteCategory(categoryId).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Category not found.");
        result.ErrorType.Should().Be(ErrorType.NotFound);
        await _unitOfWorkMock.DidNotReceive().CommitAsync();
    }
}

