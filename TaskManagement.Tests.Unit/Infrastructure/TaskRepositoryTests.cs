using Microsoft.Extensions.Caching.Memory;
using TaskManagement.Infrastructure.Repositories;

namespace TaskManagement.Tests.Unit.Infrastructure;

[TestClass]
public class TaskRepositoryTests
{
    private IMemoryCache _memoryCache;
    private TaskRepository _taskRepository;

    [TestInitialize]
    public void Setup()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _taskRepository = new TaskRepository(_memoryCache);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _memoryCache?.Dispose();
    }

    [TestMethod]
    public async Task CreateAsync_ShouldCreateTaskWithGeneratedId()
    {
        // Arrange
        var task = new Domain.Entities.Task
        {
            Title = "Test Task",
            Description = "Test Description",
            Status = Domain.Enums.TaskStatus.Pending,
            DueDate = DateTime.UtcNow.AddDays(7),
            UserId = 1,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _taskRepository.CreateAsync(task);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual("Test Task", result.Title);
        Assert.AreEqual(1, result.UserId);
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnTask_WhenTaskExists()
    {
        // Arrange
        var task = new Domain.Entities.Task
        {
            Title = "Test Task",
            Description = "Test Description",
            Status = Domain.Enums.TaskStatus.Pending,
            DueDate = DateTime.UtcNow.AddDays(7),
            UserId = 1,
            CreatedAt = DateTime.UtcNow
        };
        var createdTask = await _taskRepository.CreateAsync(task);

        // Act
        var result = await _taskRepository.GetByIdAsync(createdTask.Id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(createdTask.Id, result.Id);
        Assert.AreEqual("Test Task", result.Title);
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnNull_WhenTaskDoesNotExist()
    {
        // Act
        var result = await _taskRepository.GetByIdAsync(999);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetByUserIdAsync_ShouldReturnUserTasks()
    {
        // Arrange
        var task1 = new Domain.Entities.Task { Title = "Task 1", UserId = 1, Status = Domain.Enums.TaskStatus.Pending, DueDate = DateTime.UtcNow.AddDays(1), CreatedAt = DateTime.UtcNow };
        var task2 = new Domain.Entities.Task { Title = "Task 2", UserId = 1, Status = Domain.Enums.TaskStatus.Pending, DueDate = DateTime.UtcNow.AddDays(2), CreatedAt = DateTime.UtcNow };
        var task3 = new Domain.Entities.Task { Title = "Task 3", UserId = 2, Status = Domain.Enums.TaskStatus.Pending, DueDate = DateTime.UtcNow.AddDays(3), CreatedAt = DateTime.UtcNow };

        await _taskRepository.CreateAsync(task1);
        await _taskRepository.CreateAsync(task2);
        await _taskRepository.CreateAsync(task3);

        // Act
        var result = await _taskRepository.GetByUserIdAsync(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.All(t => t.UserId == 1));
    }

    [TestMethod]
    public async Task UpdateAsync_ShouldUpdateExistingTask()
    {
        // Arrange
        var task = new Domain.Entities.Task
        {
            Title = "Original Title",
            Description = "Original Description",
            Status = Domain.Enums.TaskStatus.Pending,
            DueDate = DateTime.UtcNow.AddDays(7),
            UserId = 1,
            CreatedAt = DateTime.UtcNow
        };
        var createdTask = await _taskRepository.CreateAsync(task);

        // Modify task
        createdTask.Title = "Updated Title";
        createdTask.Description = "Updated Description";
        createdTask.Status = Domain.Enums.TaskStatus.InProgress;
        createdTask.UpdatedAt = DateTime.UtcNow;

        // Act
        var result = await _taskRepository.UpdateAsync(createdTask);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Updated Title", result.Title);
        Assert.AreEqual("Updated Description", result.Description);
        Assert.AreEqual(Domain.Enums.TaskStatus.InProgress, result.Status);
        Assert.IsNotNull(result.UpdatedAt);
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldReturnTrue_WhenTaskExists()
    {
        // Arrange
        var task = new Domain.Entities.Task
        {
            Title = "Test Task",
            Status = Domain.Enums.TaskStatus.Pending,
            DueDate = DateTime.UtcNow.AddDays(7),
            UserId = 1,
            CreatedAt = DateTime.UtcNow
        };
        var createdTask = await _taskRepository.CreateAsync(task);

        // Act
        var result = await _taskRepository.DeleteAsync(createdTask.Id);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
    {
        // Act
        var result = await _taskRepository.DeleteAsync(999);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldRemoveTaskFromRepository()
    {
        // Arrange
        var task = new Domain.Entities.Task
        {
            Title = "Test Task",
            Status = Domain.Enums.TaskStatus.Pending,
            DueDate = DateTime.UtcNow.AddDays(7),
            UserId = 1,
            CreatedAt = DateTime.UtcNow
        };
        var createdTask = await _taskRepository.CreateAsync(task);

        // Act
        await _taskRepository.DeleteAsync(createdTask.Id);
        var deletedTask = await _taskRepository.GetByIdAsync(createdTask.Id);

        // Assert
        Assert.IsNull(deletedTask);
    }
}
