using FluentValidation.TestHelper;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Validators;

namespace TaskManagement.Tests.Unit.Application.Validators;

[TestClass]
public class TaskPatchDtoValidatorTests
{
    private TaskPatchDtoValidator _validator;

    [TestInitialize]
    public void Setup()
    {
        _validator = new TaskPatchDtoValidator();
    }

    [TestMethod]
    public void Validate_ShouldPass_WhenAllFieldsAreNull()
    {
        // Arrange
        var dto = new TaskPatchDto
        {
            Title = null,
            Description = null,
            Status = null,
            DueDate = null
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestMethod]
    public void Validate_ShouldPass_WhenOnlyTitleIsProvided()
    {
        // Arrange
        var dto = new TaskPatchDto
        {
            Title = "Updated Title",
            Description = null,
            Status = null,
            DueDate = null
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenTitleIsTooLong()
    {
        // Arrange
        var dto = new TaskPatchDto
        {
            Title = new string('a', 201), // 201 characters
            Description = null,
            Status = null,
            DueDate = null
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title cannot exceed 200 characters.");
    }

    [TestMethod]
    public void Validate_ShouldPass_WhenTitleIsEmpty()
    {
        // Note: Empty title is allowed in patch (it means don't update title)
        // Arrange
        var dto = new TaskPatchDto
        {
            Title = "",
            Description = null,
            Status = null,
            DueDate = null
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenDescriptionIsTooLong()
    {
        // Arrange
        var dto = new TaskPatchDto
        {
            Title = null,
            Description = new string('a', 1001), // 1001 characters
            Status = null,
            DueDate = null
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description cannot exceed 1000 characters.");
    }

    [TestMethod]
    public void Validate_ShouldPass_WhenDescriptionIsEmpty()
    {
        // Arrange
        var dto = new TaskPatchDto
        {
            Title = null,
            Description = "",
            Status = null,
            DueDate = null
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenStatusIsInvalid()
    {
        // Arrange
        var dto = new TaskPatchDto
        {
            Title = null,
            Description = null,
            Status = 999, // Invalid status
            DueDate = null
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status)
            .WithErrorMessage("Status must be one of: Pending (0), InProgress (1), Completed (2).");
    }

    [TestMethod]
    public void Validate_ShouldPass_WhenValidStatusIsProvided()
    {
        // Arrange
        var dto = new TaskPatchDto
        {
            Title = null,
            Description = null,
            Status = (int)Domain.Enums.TaskStatus.InProgress,
            DueDate = null
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }
}