using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

/// <summary>
/// Interface for task service
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Gets all tasks for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Collection of tasks</returns>
    Task<IEnumerable<TaskResponseDto>> GetUserTasksAsync(int userId);

    /// <summary>
    /// Gets a specific task by ID
    /// </summary>
    /// <param name="taskId">Task ID</param>
    /// <param name="userId">User ID for authorization</param>
    /// <returns>Task if found and authorized</returns>
    Task<TaskResponseDto?> GetTaskByIdAsync(int taskId, int userId);

    /// <summary>
    /// Creates a new task
    /// </summary>
    /// <param name="createDto">Task creation data</param>
    /// <param name="userId">User ID</param>
    /// <returns>Created task</returns>
    Task<TaskResponseDto> CreateTaskAsync(TaskCreateDto createDto, int userId);

    /// <summary>
    /// Updates an existing task
    /// </summary>
    /// <param name="taskId">Task ID</param>
    /// <param name="updateDto">Update data</param>
    /// <param name="userId">User ID for authorization</param>
    /// <returns>Updated task if found and authorized</returns>
    Task<TaskResponseDto?> UpdateTaskAsync(int taskId, TaskUpdateDto updateDto, int userId);

    /// <summary>
    /// Partially updates a task
    /// </summary>
    /// <param name="taskId">Task ID</param>
    /// <param name="patchDto">Patch data</param>
    /// <param name="userId">User ID for authorization</param>
    /// <returns>Updated task if found and authorized</returns>
    Task<TaskResponseDto?> PatchTaskAsync(int taskId, TaskPatchDto patchDto, int userId);

    /// <summary>
    /// Deletes a task
    /// </summary>
    /// <param name="taskId">Task ID</param>
    /// <param name="userId">User ID for authorization</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteTaskAsync(int taskId, int userId);
}
