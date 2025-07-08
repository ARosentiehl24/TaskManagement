using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Services;

/// <summary>
/// Service for handling task operations
/// </summary>
public class TaskService(ITaskRepository taskRepository, IUserRepository userRepository) : ITaskService
{
    /// <summary>
    /// Gets all tasks for a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Collection of user's tasks</returns>
    public async Task<IEnumerable<TaskResponseDto>> GetUserTasksAsync(int userId)
    {
        var tasks = await taskRepository.GetByUserIdAsync(userId);
        return tasks.Select(MapToResponseDto);
    }

    /// <summary>
    /// Gets a specific task by ID if user is authorized
    /// </summary>
    /// <param name="taskId">Task ID</param>
    /// <param name="userId">User ID for authorization</param>
    /// <returns>Task if found and user is authorized</returns>
    public async Task<TaskResponseDto?> GetTaskByIdAsync(int taskId, int userId)
    {
        var task = await taskRepository.GetByIdAsync(taskId);
        return task == null || task.UserId != userId ? null : MapToResponseDto(task);
    }

    /// <summary>
    /// Creates a new task for the user
    /// </summary>
    /// <param name="createDto">Task creation data</param>
    /// <param name="userId">User ID</param>
    /// <returns>Created task</returns>
    /// <exception cref="ArgumentException">Thrown when user doesn't exist</exception>
    public async Task<TaskResponseDto> CreateTaskAsync(TaskCreateDto createDto, int userId)
    {
        // Verify user exists
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new ArgumentException("User not found.", nameof(userId));

        var task = new Domain.Entities.Task
        {
            Title = createDto.Title,
            Description = createDto.Description,
            Status = (Domain.Enums.TaskStatus)createDto.Status,
            DueDate = createDto.DueDate,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        var createdTask = await taskRepository.CreateAsync(task);
        return MapToResponseDto(createdTask);
    }

    /// <summary>
    /// Updates an existing task if user is authorized
    /// </summary>
    /// <param name="taskId">Task ID</param>
    /// <param name="updateDto">Update data</param>
    /// <param name="userId">User ID for authorization</param>
    /// <returns>Updated task if found and authorized</returns>
    public async Task<TaskResponseDto?> UpdateTaskAsync(int taskId, TaskUpdateDto updateDto, int userId)
    {
        var existingTask = await taskRepository.GetByIdAsync(taskId);
        if (existingTask == null || existingTask.UserId != userId)
            return null;

        existingTask.Title = updateDto.Title;
        existingTask.Description = updateDto.Description;
        existingTask.Status = (Domain.Enums.TaskStatus)updateDto.Status;
        existingTask.DueDate = updateDto.DueDate;
        existingTask.UpdatedAt = DateTime.UtcNow;

        var updatedTask = await taskRepository.UpdateAsync(existingTask);
        return MapToResponseDto(updatedTask);
    }

    /// <summary>
    /// Partially updates a task if user is authorized
    /// </summary>
    /// <param name="taskId">Task ID</param>
    /// <param name="patchDto">Patch data</param>
    /// <param name="userId">User ID for authorization</param>
    /// <returns>Updated task if found and authorized</returns>
    public async Task<TaskResponseDto?> PatchTaskAsync(int taskId, TaskPatchDto patchDto, int userId)
    {
        var existingTask = await taskRepository.GetByIdAsync(taskId);
        if (existingTask == null || existingTask.UserId != userId)
            return null;

        // Update only provided fields
        if (!string.IsNullOrEmpty(patchDto.Title))
            existingTask.Title = patchDto.Title;

        if (patchDto.Description != null)
            existingTask.Description = patchDto.Description;

        if (patchDto.Status.HasValue)
            existingTask.Status = (Domain.Enums.TaskStatus)patchDto.Status.Value;

        if (patchDto.DueDate.HasValue)
            existingTask.DueDate = patchDto.DueDate.Value;

        existingTask.UpdatedAt = DateTime.UtcNow;

        var updatedTask = await taskRepository.UpdateAsync(existingTask);
        return MapToResponseDto(updatedTask);
    }

    /// <summary>
    /// Deletes a task if user is authorized
    /// </summary>
    /// <param name="taskId">Task ID</param>
    /// <param name="userId">User ID for authorization</param>
    /// <returns>True if deleted successfully</returns>
    public async Task<bool> DeleteTaskAsync(int taskId, int userId)
    {
        var existingTask = await taskRepository.GetByIdAsync(taskId);
        return existingTask == null || existingTask.UserId != userId ? false : await taskRepository.DeleteAsync(taskId);
    }

    /// <summary>
    /// Maps domain task entity to response DTO
    /// </summary>
    /// <param name="task">Domain task entity</param>
    /// <returns>Task response DTO</returns>
    private static TaskResponseDto MapToResponseDto(Domain.Entities.Task task)
    {
        return new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            DueDate = task.DueDate,
            UserId = task.UserId,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}
