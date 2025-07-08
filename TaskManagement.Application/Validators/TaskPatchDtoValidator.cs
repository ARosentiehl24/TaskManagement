using FluentValidation;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Validators;

/// <summary>
/// Validator for TaskPatchDto
/// </summary>
public class TaskPatchDtoValidator : AbstractValidator<TaskPatchDto>
{
    public TaskPatchDtoValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters.")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Status)
            .Must(BeValidTaskStatus)
            .WithMessage("Status must be one of: Pending (0), InProgress (1), Completed (2).")
            .When(x => x.Status.HasValue);

        RuleFor(x => x.DueDate)
            .NotEmpty()
            .WithMessage("Due date cannot be empty when provided.")
            .When(x => x.DueDate.HasValue);
    }

    private static bool BeValidTaskStatus(int? status)
    {
        return status.HasValue && Enum.IsDefined(typeof(TaskStatus), status.Value);
    }
}