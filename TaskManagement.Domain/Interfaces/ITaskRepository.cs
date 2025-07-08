namespace TaskManagement.Domain.Interfaces;

/// <summary>
/// Repository interface for task operations
/// </summary>
public interface ITaskRepository
{
    /// <summary>
    /// Gets all tasks for a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Collection of tasks</returns>
    Task<IEnumerable<Entities.Task>> GetByUserIdAsync(int userId);

    /// <summary>
    /// Gets a specific task by ID
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <returns>Task if found, null otherwise</returns>
    Task<Entities.Task?> GetByIdAsync(int id);

    /// <summary>
    /// Creates a new task
    /// </summary>
    /// <param name="task">Task to create</param>
    /// <returns>Created task with assigned ID</returns>
    Task<Entities.Task> CreateAsync(Entities.Task task);

    /// <summary>
    /// Updates an existing task
    /// </summary>
    /// <param name="task">Task to update</param>
    /// <returns>Updated task</returns>
    Task<Entities.Task> UpdateAsync(Entities.Task task);

    /// <summary>
    /// Deletes a task
    /// </summary>
    /// <param name="id">Task ID to delete</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteAsync(int id);
}
