using Moq;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using Task = TaskManagement.Domain.Entities.Task;

namespace TaskManagement.Tests.Unit.Application;

[TestClass]
public class TaskServiceTests
{
    private Mock<ITaskRepository> _mockTaskRepository;
    private Mock<IUserRepository> _mockUserRepository;
    private TaskService _taskService;

    [TestInitialize]
    public void Setup()
    {
        _mockTaskRepository = new Mock<ITaskRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _taskService = new TaskService(_mockTaskRepository.Object, _mockUserRepository.Object);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task GetUserTasksAsync_ShouldReturnTasks_ForExistingUser()
    {
        // Arrange
        int userId = 1;
        var tasks = new List<Task>
        {
            new() { Id = 1, Title = "Tarea 1", Description = "Desc", Status = Domain.Enums.TaskStatus.Pending, DueDate = DateTime.UtcNow, UserId = userId, CreatedAt = DateTime.UtcNow }
        };
        _mockTaskRepository.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(tasks);

        // Act
        var result = await _taskService.GetUserTasksAsync(userId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("Tarea 1", result.First().Title);
        _mockTaskRepository.Verify(r => r.GetByUserIdAsync(userId), Times.Once);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task GetTaskByIdAsync_ShouldReturnTask_WhenAuthorized()
    {
        // Arrange
        int userId = 1, taskId = 2;
        var task = new Task { Id = taskId, Title = "Tarea", Description = "Desc", Status = Domain.Enums.TaskStatus.Pending, DueDate = DateTime.UtcNow, UserId = userId, CreatedAt = DateTime.UtcNow };
        _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(task);

        // Act
        var result = await _taskService.GetTaskByIdAsync(taskId, userId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(taskId, result.Id);
        _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task GetTaskByIdAsync_ShouldReturnNull_WhenNotAuthorized()
    {
        // Arrange
        int userId = 1, taskId = 2;
        var task = new Task { Id = taskId, UserId = 99 };
        _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(task);

        // Act
        var result = await _taskService.GetTaskByIdAsync(taskId, userId);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task CreateTaskAsync_ShouldCreateTask_WhenUserExists()
    {
        // Arrange
        int userId = 1;
        var createDto = new TaskCreateDto { Title = "Nueva", Description = "Desc", Status = 0, DueDate = DateTime.UtcNow };
        var user = new User { Id = userId, Username = "usuario" };
        var createdTask = new Task { Id = 10, Title = "Nueva", Description = "Desc", Status = Domain.Enums.TaskStatus.Pending, DueDate = createDto.DueDate, UserId = userId, CreatedAt = DateTime.UtcNow };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockTaskRepository.Setup(r => r.CreateAsync(It.IsAny<Task>())).ReturnsAsync(createdTask);

        // Act
        var result = await _taskService.CreateTaskAsync(createDto, userId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Nueva", result.Title);
        _mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _mockTaskRepository.Verify(r => r.CreateAsync(It.IsAny<Task>()), Times.Once);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task CreateTaskAsync_ShouldThrow_WhenUserNotFound()
    {
        // Arrange
        int userId = 1;
        var createDto = new TaskCreateDto { Title = "Nueva", Description = "Desc", Status = 0, DueDate = DateTime.UtcNow };
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null);

        // Act & Assert
        var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            await _taskService.CreateTaskAsync(createDto, userId));
        Assert.AreEqual("User not found. (Parameter 'userId')", ex.Message);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task UpdateTaskAsync_ShouldUpdate_WhenAuthorized()
    {
        // Arrange
        int userId = 1, taskId = 2;
        var updateDto = new TaskUpdateDto { Title = "Actualizada", Description = "Desc", Status = 1, DueDate = DateTime.UtcNow };
        var existingTask = new Task { Id = taskId, Title = "Vieja", Description = "Vieja", Status = Domain.Enums.TaskStatus.Pending, DueDate = DateTime.UtcNow, UserId = userId, CreatedAt = DateTime.UtcNow };
        var updatedTask = new Task { Id = taskId, Title = "Actualizada", Description = "Desc", Status = Domain.Enums.TaskStatus.InProgress, DueDate = updateDto.DueDate, UserId = userId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

        _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(existingTask);
        _mockTaskRepository.Setup(r => r.UpdateAsync(It.IsAny<Task>())).ReturnsAsync(updatedTask);

        // Act
        var result = await _taskService.UpdateTaskAsync(taskId, updateDto, userId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Actualizada", result.Title);
        _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
        _mockTaskRepository.Verify(r => r.UpdateAsync(It.IsAny<Task>()), Times.Once);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task UpdateTaskAsync_ShouldReturnNull_WhenNotAuthorized()
    {
        // Arrange
        int userId = 1, taskId = 2;
        var updateDto = new TaskUpdateDto { Title = "Actualizada", Description = "Desc", Status = 1, DueDate = DateTime.UtcNow };
        var existingTask = new Task { Id = taskId, UserId = 99 };

        _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(existingTask);

        // Act
        var result = await _taskService.UpdateTaskAsync(taskId, updateDto, userId);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task PatchTaskAsync_ShouldPatchFields_WhenAuthorized()
    {
        // Arrange
        int userId = 1, taskId = 2;
        var patchDto = new TaskPatchDto { Title = "Parcheada" };
        var existingTask = new Task { Id = taskId, Title = "Vieja", UserId = userId, CreatedAt = DateTime.UtcNow };
        var updatedTask = new Task { Id = taskId, Title = "Parcheada", UserId = userId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

        _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(existingTask);
        _mockTaskRepository.Setup(r => r.UpdateAsync(It.IsAny<Task>())).ReturnsAsync(updatedTask);

        // Act
        var result = await _taskService.PatchTaskAsync(taskId, patchDto, userId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Parcheada", result.Title);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task PatchTaskAsync_ShouldReturnNull_WhenNotAuthorized()
    {
        // Arrange
        int userId = 1, taskId = 2;
        var patchDto = new TaskPatchDto { Title = "Parcheada" };
        var existingTask = new Task { Id = taskId, UserId = 99 };

        _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(existingTask);

        // Act
        var result = await _taskService.PatchTaskAsync(taskId, patchDto, userId);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task DeleteTaskAsync_ShouldDelete_WhenAuthorized()
    {
        // Arrange
        int userId = 1, taskId = 2;
        var existingTask = new Task { Id = taskId, UserId = userId };
        _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(existingTask);
        _mockTaskRepository.Setup(r => r.DeleteAsync(taskId)).ReturnsAsync(true);

        // Act
        var result = await _taskService.DeleteTaskAsync(taskId, userId);

        // Assert
        Assert.IsTrue(result);
        _mockTaskRepository.Verify(r => r.DeleteAsync(taskId), Times.Once);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task DeleteTaskAsync_ShouldReturnFalse_WhenNotAuthorized()
    {
        // Arrange
        int userId = 1, taskId = 2;
        var existingTask = new Task { Id = taskId, UserId = 99 };
        _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(existingTask);

        // Act
        var result = await _taskService.DeleteTaskAsync(taskId, userId);

        // Assert
        Assert.IsFalse(result);
        _mockTaskRepository.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
    }
}