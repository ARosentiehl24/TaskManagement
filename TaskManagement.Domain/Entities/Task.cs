namespace TaskManagement.Domain.Entities;

/// <summary>
/// Represents a task in the system
/// </summary>
public class Task
{
    /// <summary>
    /// Unique identifier for the task
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Title of the task
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Description of the task
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the task
    /// </summary>
    public Enums.TaskStatus Status { get; set; }

    /// <summary>
    /// Due date for the task
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// ID of the user who owns this task
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Date and time when the task was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the task was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}