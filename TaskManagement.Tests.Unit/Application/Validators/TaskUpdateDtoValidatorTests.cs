using FluentValidation.TestHelper;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Validators;

namespace TaskManagement.Tests.Unit.Application.Validators;

[TestClass]
public class TaskUpdateDtoValidatorTests
{
    private TaskUpdateDtoValidator _validator;

    [TestInitialize]
    public void Setup()
    {
        _validator = new TaskUpdateDtoValidator();
    }

    [TestMethod]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        // Arrange
        var dto = new TaskUpdateDto
        {
            Title = "Updated Task Title",
            Description = "Updated task description",
            Status = (int)Domain.Enums.TaskStatus.InProgress,
            DueDate = DateTime.Now.AddDays(7)
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenTitleIsEmpty()
    {
        // Arrange
        var dto = new TaskUpdateDto
        {
            Title = "",
            Description = "Valid description",
            Status = (int)Domain.Enums.TaskStatus.InProgress,
            DueDate = DateTime.Now.AddDays(7)
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required.");
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenDueDateIsEmpty()
    {
        // Arrange
        var dto = new TaskUpdateDto
        {
            Title = "Valid Title",
            Description = "Valid description",
            Status = (int)Domain.Enums.TaskStatus.InProgress,
            DueDate = default // Empty date
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DueDate)
            .WithErrorMessage("Due date is required.");
    }

    [TestMethod]
    public void Validate_ShouldPass_WhenDescriptionIsEmpty()
    {
        // Arrange
        var dto = new TaskUpdateDto
        {
            Title = "Valid Title",
            Description = "",
            Status = (int)Domain.Enums.TaskStatus.InProgress,
            DueDate = DateTime.Now.AddDays(7)
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [TestMethod]
    public void Validate_ShouldPass_WhenDueDateIsInThePast()
    {
        // Note: Unlike create, update doesn't require future date
        // Arrange
        var dto = new TaskUpdateDto
        {
            Title = "Valid Title",
            Description = "Valid description",
            Status = (int)Domain.Enums.TaskStatus.Completed,
            DueDate = DateTime.Now.AddDays(-1) // Past date is allowed for updates
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DueDate);
    }
}
