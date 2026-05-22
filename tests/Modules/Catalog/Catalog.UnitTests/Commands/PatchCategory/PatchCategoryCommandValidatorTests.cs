using Catalog.Application.Features.Commands.PatchCategory;
using Catalog.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Catalog.UnitTests.Commands.PatchCategory;

public sealed class PatchCategoryCommandValidatorTests
{
    private readonly PatchCategoryCommandValidator _validator = new();

    [Fact]
    public void Validator_ShouldHaveError_WhenIdIsEmpty()
    {
        // Arrange
        var command = new PatchCategoryCommand(Guid.Empty, "Name", null, null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Id" && e.ErrorMessage == "Id is required.");
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenNameIsTooLong()
    {
        // Arrange
        var command = new PatchCategoryCommand(Guid.NewGuid(), new string('a', 101), null, null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Name" && e.ErrorMessage == "Name cannot be longer than 100 characters.");
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenTypeIsInvalid()
    {
        // Arrange
        var command = new PatchCategoryCommand(Guid.NewGuid(), "Name", (CatalogType)999, null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Type" && e.ErrorMessage == "Invalid category type.");
    }

    [Fact]
    public void Validator_ShouldBeValid_WhenCommandIsCorrect()
    {
        // Arrange
        var command = new PatchCategoryCommand(Guid.NewGuid(), "New Name", CatalogType.Expense, true);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validator_ShouldBeValid_WhenOnlyIdIsProvided()
    {
        // Arrange
        var command = new PatchCategoryCommand(Guid.NewGuid(), null, null, null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}

