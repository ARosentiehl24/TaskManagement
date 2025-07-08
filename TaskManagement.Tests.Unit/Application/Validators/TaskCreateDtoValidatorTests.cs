using FluentValidation.TestHelper;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Validators;

namespace TaskManagement.Tests.Unit.Application.Validators;

[TestClass]
public class TaskCreateDtoValidatorTests
{
    private TaskCreateDtoValidator _validator;

    [TestInitialize]
    public void Setup()
    {
        _validator = new TaskCreateDtoValidator();
    }

    [TestMethod]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        // Arrange
        var dto = new TaskCreateDto
        {
            Title = "Valid Task Title",
            Description = "Valid task description",
            Status = (int)Domain.Enums.TaskStatus.Pending,
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
        var dto = new TaskCreateDto
        {
            Title = "",
            Description = "Valid description",
            Status = (int)Domain.Enums.TaskStatus.Pending,
            DueDate = DateTime.Now.AddDays(7)
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required.");
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenTitleIsTooLong()
    {
        // Arrange
        var dto = new TaskCreateDto
        {
            Title = new string('a', 201), // 201 characters
            Description = "Valid description",
            Status = (int)Domain.Enums.TaskStatus.Pending,
            DueDate = DateTime.Now.AddDays(7)
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title cannot exceed 200 characters.");
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenDescriptionIsTooLong()
    {
        // Arrange
        var dto = new TaskCreateDto
        {
            Title = "Valid Title",
            Description = new string('a', 1001), // 1001 characters
            Status = (int)Domain.Enums.TaskStatus.Pending,
            DueDate = DateTime.Now.AddDays(7)
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
        var dto = new TaskCreateDto
        {
            Title = "Valid Title",
            Description = "",
            Status = (int)Domain.Enums.TaskStatus.Pending,
            DueDate = DateTime.Now.AddDays(7)
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
        var dto = new TaskCreateDto
        {
            Title = "Valid Title",
            Description = "Valid description",
            Status = 999, // Invalid status
            DueDate = DateTime.Now.AddDays(7)
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status)
            .WithErrorMessage("Status must be one of: Pending (0), InProgress (1), Completed (2).");
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenDueDateIsInThePast()
    {
        // Arrange
        var dto = new TaskCreateDto
        {
            Title = "Valid Title",
            Description = "Valid description",
            Status = (int)Domain.Enums.TaskStatus.Pending,
            DueDate = DateTime.Now.AddDays(-1) // Past date
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DueDate)
            .WithErrorMessage("Due date must be in the future.");
    }

    [TestMethod]
    public void Validate_ShouldPass_WhenAllStatusValuesAreValid()
    {
        // Test all valid status values
        var validStatuses = new[] { (int)Domain.Enums.TaskStatus.Pending, (int)Domain.Enums.TaskStatus.InProgress, (int)Domain.Enums.TaskStatus.Completed };

        foreach (var status in validStatuses)
        {
            // Arrange
            var dto = new TaskCreateDto
            {
                Title = "Valid Title",
                Description = "Valid description",
                Status = status,
                DueDate = DateTime.Now.AddDays(7)
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Status);
        }
    }

    [TestMethod]
    public void Validate_ShouldPass_WhenDueDateIsExactlyNow()
    {
        // Arrange
        var dto = new TaskCreateDto
        {
            Title = "Valid Title",
            Description = "Valid description",
            Status = (int)Domain.Enums.TaskStatus.Pending,
            DueDate = DateTime.Now.AddMinutes(1) // Just slightly in the future
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DueDate);
    }
}