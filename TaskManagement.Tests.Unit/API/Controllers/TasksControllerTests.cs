using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Server.Controllers;

namespace TaskManagement.Tests.Unit.API.Controllers;

[TestClass]
public class TasksControllerTests
{
    private Mock<ITaskService> _mockTaskService;
    private TasksController _controller;

    [TestInitialize]
    public void Setup()
    {
        _mockTaskService = new Mock<ITaskService>();
        _controller = new TasksController(_mockTaskService.Object);

        // Setup default user context
        SetupUserContext(1);
    }

    private void SetupUserContext(int userId)
    {
        var claims = new[]
        {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, "testuser")
            };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [TestMethod]
    public async Task GetTasks_ShouldReturnOkResult_WhenUserIsAuthenticated()
    {
        // Arrange
        var userId = 1;
        var tasks = new List<TaskResponseDto>
            {
                new() {
                    Id = 1,
                    Title = "Task 1",
                    Description = "Description 1",
                    Status = "Pending",
                    UserId = userId
                },
                new() {
                    Id = 2,
                    Title = "Task 2",
                    Description = "Description 2",
                    Status = "InProgress",
                    UserId = userId
                }
            };

        _mockTaskService.Setup(x => x.GetUserTasksAsync(userId)).ReturnsAsync(tasks);

        // Act
        var result = await _controller.GetTasks();

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.AreEqual(tasks, okResult.Value);

        _mockTaskService.Verify(x => x.GetUserTasksAsync(userId), Times.Once);
    }

    [TestMethod]
    public async Task GetTasks_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var result = await _controller.GetTasks();

        // Assert
        Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        _mockTaskService.Verify(x => x.GetUserTasksAsync(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public async Task GetTask_ShouldReturnOkResult_WhenTaskExistsAndUserIsAuthorized()
    {
        // Arrange
        var taskId = 1;
        var userId = 1;
        var task = new TaskResponseDto
        {
            Id = taskId,
            Title = "Test Task",
            Description = "Test Description",
            Status = "Pending",
            UserId = userId
        };

        _mockTaskService.Setup(x => x.GetTaskByIdAsync(taskId, userId)).ReturnsAsync(task);

        // Act
        var result = await _controller.GetTask(taskId);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.AreEqual(task, okResult.Value);

        _mockTaskService.Verify(x => x.GetTaskByIdAsync(taskId, userId), Times.Once);
    }

    [TestMethod]
    public async Task GetTask_ShouldReturnNotFound_WhenTaskDoesNotExistOrUserNotAuthorized()
    {
        // Arrange
        var taskId = 999;
        var userId = 1;

        _mockTaskService.Setup(x => x.GetTaskByIdAsync(taskId, userId)).ReturnsAsync((TaskResponseDto)null);

        // Act
        var result = await _controller.GetTask(taskId);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        _mockTaskService.Verify(x => x.GetTaskByIdAsync(taskId, userId), Times.Once);
    }

    [TestMethod]
    public async Task CreateTask_ShouldReturnCreatedResult_WhenTaskIsCreatedSuccessfully()
    {
        // Arrange
        var userId = 1;
        var createDto = new TaskCreateDto
        {
            Title = "New Task",
            Description = "New Description",
            Status = 0,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        var createdTask = new TaskResponseDto
        {
            Id = 1,
            Title = createDto.Title,
            Description = createDto.Description,
            Status = "Pending",
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _mockTaskService.Setup(x => x.CreateTaskAsync(createDto, userId)).ReturnsAsync(createdTask);

        // Act
        var result = await _controller.CreateTask(createDto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
        var createdResult = result as CreatedAtActionResult;
        Assert.AreEqual(createdTask, createdResult.Value);
        Assert.AreEqual(nameof(TasksController.GetTask), createdResult.ActionName);

        _mockTaskService.Verify(x => x.CreateTaskAsync(createDto, userId), Times.Once);
    }

    [TestMethod]
    public async Task CreateTask_ShouldReturnBadRequest_WhenCreationFails()
    {
        // Arrange
        var userId = 1;
        var createDto = new TaskCreateDto
        {
            Title = "New Task",
            Description = "New Description",
            Status = 0,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        var errorMessage = "User not found.";
        _mockTaskService.Setup(x => x.CreateTaskAsync(createDto, userId))
            .ThrowsAsync(new ArgumentException(errorMessage));

        // Act
        var result = await _controller.CreateTask(createDto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        var badRequestResult = result as BadRequestObjectResult;

        var errorResponse = badRequestResult.Value;
        var errorProperty = errorResponse.GetType().GetProperty("error");
        Assert.AreEqual(errorMessage, errorProperty.GetValue(errorResponse));

        _mockTaskService.Verify(x => x.CreateTaskAsync(createDto, userId), Times.Once);
    }

    [TestMethod]
    public async Task UpdateTask_ShouldReturnOkResult_WhenTaskIsUpdatedSuccessfully()
    {
        // Arrange
        var taskId = 1;
        var userId = 1;
        var updateDto = new TaskUpdateDto
        {
            Title = "Updated Task",
            Description = "Updated Description",
            Status = 1,
            DueDate = DateTime.UtcNow.AddDays(10)
        };

        var updatedTask = new TaskResponseDto
        {
            Id = taskId,
            Title = updateDto.Title,
            Description = updateDto.Description,
            Status = "InProgress",
            UserId = userId,
            UpdatedAt = DateTime.UtcNow
        };

        _mockTaskService.Setup(x => x.UpdateTaskAsync(taskId, updateDto, userId)).ReturnsAsync(updatedTask);

        // Act
        var result = await _controller.UpdateTask(taskId, updateDto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.AreEqual(updatedTask, okResult.Value);

        _mockTaskService.Verify(x => x.UpdateTaskAsync(taskId, updateDto, userId), Times.Once);
    }

    [TestMethod]
    public async Task UpdateTask_ShouldReturnNotFound_WhenTaskDoesNotExistOrUserNotAuthorized()
    {
        // Arrange
        var taskId = 999;
        var userId = 1;
        var updateDto = new TaskUpdateDto
        {
            Title = "Updated Task",
            Description = "Updated Description",
            Status = 1,
            DueDate = DateTime.UtcNow.AddDays(10)
        };

        _mockTaskService.Setup(x => x.UpdateTaskAsync(taskId, updateDto, userId)).ReturnsAsync((TaskResponseDto)null);

        // Act
        var result = await _controller.UpdateTask(taskId, updateDto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        _mockTaskService.Verify(x => x.UpdateTaskAsync(taskId, updateDto, userId), Times.Once);
    }

    [TestMethod]
    public async Task PatchTask_ShouldReturnOkResult_WhenTaskIsPatchedSuccessfully()
    {
        // Arrange
        var taskId = 1;
        var userId = 1;
        var patchDto = new TaskPatchDto
        {
            Title = "Patched Title",
            Status = 2
        };

        var patchedTask = new TaskResponseDto
        {
            Id = taskId,
            Title = patchDto.Title,
            Status = "Completed",
            UserId = userId,
            UpdatedAt = DateTime.UtcNow
        };

        _mockTaskService.Setup(x => x.PatchTaskAsync(taskId, patchDto, userId)).ReturnsAsync(patchedTask);

        // Act
        var result = await _controller.PatchTask(taskId, patchDto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.AreEqual(patchedTask, okResult.Value);

        _mockTaskService.Verify(x => x.PatchTaskAsync(taskId, patchDto, userId), Times.Once);
    }

    [TestMethod]
    public async Task DeleteTask_ShouldReturnNoContent_WhenTaskIsDeletedSuccessfully()
    {
        // Arrange
        var taskId = 1;
        var userId = 1;

        _mockTaskService.Setup(x => x.DeleteTaskAsync(taskId, userId)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteTask(taskId);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        _mockTaskService.Verify(x => x.DeleteTaskAsync(taskId, userId), Times.Once);
    }

    [TestMethod]
    public async Task DeleteTask_ShouldReturnNotFound_WhenTaskDoesNotExistOrUserNotAuthorized()
    {
        // Arrange
        var taskId = 999;
        var userId = 1;

        _mockTaskService.Setup(x => x.DeleteTaskAsync(taskId, userId)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteTask(taskId);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        _mockTaskService.Verify(x => x.DeleteTaskAsync(taskId, userId), Times.Once);
    }
}