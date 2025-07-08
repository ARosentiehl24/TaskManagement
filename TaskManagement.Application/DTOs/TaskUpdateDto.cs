namespace TaskManagement.Application.DTOs;

/// <summary>
/// DTO for updating an existing task
/// </summary>
public class TaskUpdateDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Status { get; set; }

    public DateTime DueDate { get; set; }
}
