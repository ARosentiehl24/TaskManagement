using FluentValidation;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Validators;

/// <summary>
/// Validator for TaskCreateDto
/// </summary>
public class TaskCreateDtoValidator : AbstractValidator<TaskCreateDto>
{
    public TaskCreateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.Status)
            .Must(BeValidTaskStatus)
            .WithMessage("Status must be one of: Pending (0), InProgress (1), Completed (2).");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.Now)
            .WithMessage("Due date must be in the future.");
    }

    private static bool BeValidTaskStatus(int status)
    {
        return Enum.IsDefined(typeof(TaskStatus), status);
    }
}