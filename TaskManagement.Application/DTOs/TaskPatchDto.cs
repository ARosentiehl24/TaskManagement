namespace TaskManagement.Application.DTOs;

/// <summary>
/// DTO for partially updating a task
/// </summary>
public class TaskPatchDto
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? Status { get; set; }

    public DateTime? DueDate { get; set; }
}