using FluentAssertions;
using TaskManagement.Service.Features.Tasks.Commands;
using TaskManagement.Service.Features.Tasks.Validators;

namespace TaskManagement.Tests.Unit.Validators;

/// <summary>
/// Unit tests for CreateTaskCommandValidator.
/// </summary>
public class CreateTaskCommandValidatorTests
{
    private readonly CreateTaskCommandValidator _validator;

    public CreateTaskCommandValidatorTests()
    {
        _validator = new CreateTaskCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Valid Task Title",
            Description = "Valid Description",
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyTitle_ShouldFail()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = string.Empty,
            Description = "Description"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_TitleTooLong_ShouldFail()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = new string('A', 201), // Exceeds 200 character limit
            Description = "Description"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title" && e.ErrorMessage.Contains("200"));
    }

    [Fact]
    public void Validate_DescriptionTooLong_ShouldFail()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Valid Title",
            Description = new string('A', 1001) // Exceeds 1000 character limit
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Validate_DueDateInPast_ShouldFail()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Valid Title",
            Description = "Description",
            DueDate = DateTime.UtcNow.AddDays(-1) // Past date
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DueDate");
    }

    [Fact]
    public void Validate_NullDescription_ShouldPass()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Valid Title",
            Description = null
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_NullDueDate_ShouldPass()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Valid Title",
            Description = "Description",
            DueDate = null
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}

