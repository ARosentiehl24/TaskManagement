using Microsoft.Extensions.Caching.Memory;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of task repository
/// </summary>
public class TaskRepository : ITaskRepository
{
    private readonly IMemoryCache _memoryCache;
    private const string TasksKey = "Tasks";
    private const string TaskIdCounterKey = "TaskIdCounter";

    public TaskRepository(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        InitializeTasks();
    }

    /// <summary>
    /// Gets all tasks for a specific user from memory cache
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Collection of user's tasks</returns>
    public Task<IEnumerable<Domain.Entities.Task>> GetByUserIdAsync(int userId)
    {
        var tasks = GetTasks();
        var userTasks = tasks.Where(t => t.UserId == userId).ToList();
        return Task.FromResult<IEnumerable<Domain.Entities.Task>>(userTasks);
    }

    /// <summary>
    /// Gets task by ID from memory cache
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <returns>Task if found</returns>
    public Task<Domain.Entities.Task?> GetByIdAsync(int id)
    {
        var tasks = GetTasks();
        var task = tasks.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(task);
    }

    /// <summary>
    /// Creates a new task in memory cache
    /// </summary>
    /// <param name="task">Task to create</param>
    /// <returns>Created task with assigned ID</returns>
    public Task<Domain.Entities.Task> CreateAsync(Domain.Entities.Task task)
    {
        var tasks = GetTasks();
        var nextId = GetNextTaskId();

        task.Id = nextId;
        tasks.Add(task);

        _memoryCache.Set(TasksKey, tasks);
        _memoryCache.Set(TaskIdCounterKey, nextId);

        return Task.FromResult(task);
    }

    /// <summary>
    /// Updates an existing task in memory cache
    /// </summary>
    /// <param name="task">Task to update</param>
    /// <returns>Updated task</returns>
    public Task<Domain.Entities.Task> UpdateAsync(Domain.Entities.Task task)
    {
        var tasks = GetTasks();
        var existingTaskIndex = tasks.FindIndex(t => t.Id == task.Id);

        if (existingTaskIndex >= 0)
        {
            tasks[existingTaskIndex] = task;
            _memoryCache.Set(TasksKey, tasks);
        }

        return Task.FromResult(task);
    }

    /// <summary>
    /// Deletes a task from memory cache
    /// </summary>
    /// <param name="id">Task ID to delete</param>
    /// <returns>True if deleted successfully</returns>
    public Task<bool> DeleteAsync(int id)
    {
        var tasks = GetTasks();
        var taskToRemove = tasks.FirstOrDefault(t => t.Id == id);

        if (taskToRemove != null)
        {
            tasks.Remove(taskToRemove);
            _memoryCache.Set(TasksKey, tasks);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    /// <summary>
    /// Gets all tasks from memory cache or initializes empty list
    /// </summary>
    /// <returns>List of tasks</returns>
    private List<Domain.Entities.Task> GetTasks()
    {
        return _memoryCache.GetOrCreate(TasksKey, factory => new List<Domain.Entities.Task>()) ?? new List<Domain.Entities.Task>();
    }

    /// <summary>
    /// Gets and increments task ID counter
    /// </summary>
    /// <returns>Next available task ID</returns>
    private int GetNextTaskId()
    {
        var currentId = _memoryCache.GetOrCreate(TaskIdCounterKey, factory => 0);
        return currentId + 1;
    }

    /// <summary>
    /// Initializes demo tasks in memory cache
    /// </summary>
    private void InitializeTasks()
    {
        if (_memoryCache.TryGetValue(TasksKey, out _))
            return; // Already initialized

        var tasks = new List<Domain.Entities.Task>();
        _memoryCache.Set(TasksKey, tasks);
        _memoryCache.Set(TaskIdCounterKey, 0);
    }
}
