using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Abstractions.Persistance;
using Catalog.Application.Features.Commands.PatchCategory;
using Catalog.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedKernel.Common;
using Xunit;

namespace Catalog.UnitTests.Commands.PatchCategory;

public class PatchCategoryCommandHandlerTests
{
    private readonly ICatalogRepository _catalogRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ILogger<PatchCategoryCommandHandler> _loggerMock;
    private readonly PatchCategoryCommandHandler _handler;

    public PatchCategoryCommandHandlerTests()
    {
        _catalogRepositoryMock = Substitute.For<ICatalogRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<PatchCategoryCommandHandler>>();

        _handler = new PatchCategoryCommandHandler(
            _unitOfWorkMock,
            _catalogRepositoryMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCategoryIsPatched()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = Catalog.Domain.Entities.Catalog.Create(Guid.NewGuid(), "Old Name", CatalogType.Expense).Value;
        var command = new PatchCategoryCommand(categoryId, "New Name", null, null);

        _catalogRepositoryMock.GetCategoryById(categoryId).Returns(category);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        category!.Name.Should().Be("New Name");
        await _catalogRepositoryMock.Received(1).PatchCategory(category);
        await _unitOfWorkMock.Received(1).CommitAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCategoryNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new PatchCategoryCommand(categoryId, "New Name", null, null);

        _catalogRepositoryMock.GetCategoryById(categoryId).Returns((Catalog.Domain.Entities.Catalog?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Category not found.");
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPatchValidationFails()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = Catalog.Domain.Entities.Catalog.Create(Guid.NewGuid(), "Old Name", CatalogType.Expense).Value;
        // Name too long (> 100)
        var command = new PatchCategoryCommand(categoryId, new string('a', 101), null, null);

        _catalogRepositoryMock.GetCategoryById(categoryId).Returns(category);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("The field name cannot be longer than 100 characters.");
        await _unitOfWorkMock.DidNotReceive().CommitAsync();
    }
}
