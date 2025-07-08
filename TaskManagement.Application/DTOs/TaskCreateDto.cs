namespace TaskManagement.Application.DTOs;

/// <summary>
/// DTO for creating a new task
/// </summary>
public class TaskCreateDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Status { get; set; }

    public DateTime DueDate { get; set; }
}
