using Catalog.Application.Features.Commands.DeleteCategory;
using FluentAssertions;
using Xunit;

namespace Catalog.UnitTests.Commands.DeleteCategory;

public class DeleteCategoryCommandValidatorTests
{
    private readonly DeleteCategoryCommandValidator _validator = new();

    [Fact]
    public void Validator_ShouldHaveError_WhenIdIsEmpty()
    {
        // Arrange
        var command = new DeleteCategoryCommand(Guid.Empty);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Id" && e.ErrorMessage == "Id is required.");
    }

    [Fact]
    public void Validator_ShouldBeValid_WhenIdIsProvided()
    {
        // Arrange
        var command = new DeleteCategoryCommand(Guid.NewGuid());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
