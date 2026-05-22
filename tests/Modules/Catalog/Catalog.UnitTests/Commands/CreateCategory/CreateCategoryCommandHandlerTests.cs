using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Abstractions.Persistance;
using Catalog.Application.Features.Commands.CreateCategory;
using Catalog.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using SharedKernel.Common;
using Xunit;

namespace Catalog.UnitTests.Commands.CreateCategory;

public class CreateCategoryCommandHandlerTests
{
    private readonly ICatalogRepository _catalogRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ICurrentUser _currentUserMock;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _catalogRepositoryMock = Substitute.For<ICatalogRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _currentUserMock = Substitute.For<ICurrentUser>();

        _handler = new CreateCategoryCommandHandler(
            _catalogRepositoryMock,
            _unitOfWorkMock,
            _currentUserMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCategoryIsCreated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateCategoryCommand("Test Category", CatalogType.Expense);
        _currentUserMock.UserId.Returns(userId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        await _catalogRepositoryMock.Received(1).CreateCategory(Arg.Is<Catalog.Domain.Entities.Catalog>(c => 
            c.Name == command.Name && 
            c.Type == command.Type && 
            c.UserId == userId));
        await _unitOfWorkMock.Received(1).CommitAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenDomainValidationFails()
    {
        // Arrange
        var command = new CreateCategoryCommand("", CatalogType.Expense); // Name is mandatory
        _currentUserMock.UserId.Returns(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("The field name is mandatory.");
        await _catalogRepositoryMock.DidNotReceive().CreateCategory(Arg.Any<Catalog.Domain.Entities.Catalog>());
        await _unitOfWorkMock.DidNotReceive().CommitAsync();
    }
}
