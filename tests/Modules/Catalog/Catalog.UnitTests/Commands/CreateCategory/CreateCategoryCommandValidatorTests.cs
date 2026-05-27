using Catalog.Application.Features.Commands.CreateCategory;
using Catalog.Domain.Enums;
using FluentAssertions;

namespace Catalog.UnitTests.Commands.CreateCategory;

public sealed class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator = new();

    [Fact]
    public void Validator_ShouldHaveError_WhenNameIsEmpty()
    {
        // Arrange
        var command = new CreateCategoryCommand("", CatalogType.Expense);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Name" && e.ErrorMessage == "Name is required.");
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenNameIsTooLong()
    {
        // Arrange
        var command = new CreateCategoryCommand(new string('a', 101), CatalogType.Expense);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Name" && e.ErrorMessage == "Name cannot be longer than 100 characters.");
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenTypeIsNone()
    {
        // Arrange
        var command = new CreateCategoryCommand("Test", CatalogType.None);

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
        var command = new CreateCategoryCommand("Test Category", CatalogType.Income);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}

