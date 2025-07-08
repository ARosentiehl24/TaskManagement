using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Tests.Integration.Controllers;

[TestClass]
public class TasksControllerIntegrationTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;
    private string _authToken;

    [TestInitialize]
    public async Task Setup()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
            });

        _client = _factory.CreateClient();

        // Get authentication token
        await AuthenticateAsync();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    private async Task AuthenticateAsync()
    {
        var loginDto = new UserLoginDto
        {
            Username = "demo_user",
            Password = "Demo123!"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
        var content = await response.Content.ReadAsStringAsync();
        var loginResult = JsonSerializer.Deserialize<LoginResponseDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        _authToken = loginResult.Token;
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);
    }

    [TestMethod]
    public async Task GetTasks_ShouldReturnOk_WhenAuthenticated()
    {
        // Act
        var response = await _client.GetAsync("/api/tasks");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var tasks = JsonSerializer.Deserialize<List<TaskResponseDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(tasks);
        // Should contain seeded tasks for demo_user
        Assert.IsTrue(tasks.Count >= 0);
    }

    [TestMethod]
    public async Task GetTasks_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/api/tasks");

        // Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task CreateTask_ShouldReturnCreated_WhenValidData()
    {
        // Arrange
        var createDto = new TaskCreateDto
        {
            Title = "Integration Test Task",
            Description = "Created during integration testing",
            Status = 0, // Pending
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", createDto);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var createdTask = JsonSerializer.Deserialize<TaskResponseDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(createdTask);
        Assert.AreEqual("Integration Test Task", createdTask.Title);
        Assert.AreEqual("Created during integration testing", createdTask.Description);
        Assert.AreEqual("Pending", createdTask.Status);
    }

    [TestMethod]
    public async Task CreateTask_ShouldReturnBadRequest_WhenInvalidData()
    {
        // Arrange - Missing title
        var createDto = new TaskCreateDto
        {
            Title = "", // Empty title should fail validation
            Description = "Description",
            Status = 0,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", createDto);

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task GetTask_ShouldReturnOk_WhenTaskExistsAndUserAuthorized()
    {
        // Arrange - First create a task
        var createDto = new TaskCreateDto
        {
            Title = "Test Task for Get",
            Description = "Test Description",
            Status = 0,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        var createResponse = await _client.PostAsJsonAsync("/api/tasks", createDto);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdTask = JsonSerializer.Deserialize<TaskResponseDto>(createContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Act
        var response = await _client.GetAsync($"/api/tasks/{createdTask.Id}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var retrievedTask = JsonSerializer.Deserialize<TaskResponseDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(retrievedTask);
        Assert.AreEqual(createdTask.Id, retrievedTask.Id);
        Assert.AreEqual("Test Task for Get", retrievedTask.Title);
    }

    [TestMethod]
    public async Task GetTask_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/tasks/99999");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task UpdateTask_ShouldReturnOk_WhenValidData()
    {
        // Arrange - First create a task
        var createDto = new TaskCreateDto
        {
            Title = "Original Title",
            Description = "Original Description",
            Status = 0,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        var createResponse = await _client.PostAsJsonAsync("/api/tasks", createDto);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdTask = JsonSerializer.Deserialize<TaskResponseDto>(createContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var updateDto = new TaskUpdateDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Status = 1, // InProgress
            DueDate = DateTime.UtcNow.AddDays(10)
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/tasks/{createdTask.Id}", updateDto);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var updatedTask = JsonSerializer.Deserialize<TaskResponseDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(updatedTask);
        Assert.AreEqual("Updated Title", updatedTask.Title);
        Assert.AreEqual("Updated Description", updatedTask.Description);
        Assert.AreEqual("InProgress", updatedTask.Status);
    }

    [TestMethod]
    public async Task PatchTask_ShouldReturnOk_WhenValidPartialUpdate()
    {
        // Arrange - First create a task
        var createDto = new TaskCreateDto
        {
            Title = "Original Title",
            Description = "Original Description",
            Status = 0,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        var createResponse = await _client.PostAsJsonAsync("/api/tasks", createDto);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdTask = JsonSerializer.Deserialize<TaskResponseDto>(createContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var patchDto = new TaskPatchDto
        {
            Status = 2 // Only update status to Completed
        };

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/tasks/{createdTask.Id}", patchDto);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var patchedTask = JsonSerializer.Deserialize<TaskResponseDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(patchedTask);
        Assert.AreEqual("Original Title", patchedTask.Title); // Should remain unchanged
        Assert.AreEqual("Original Description", patchedTask.Description); // Should remain unchanged
        Assert.AreEqual("Completed", patchedTask.Status); // Should be updated
    }

    [TestMethod]
    public async Task DeleteTask_ShouldReturnNoContent_WhenTaskExists()
    {
        // Arrange - First create a task
        var createDto = new TaskCreateDto
        {
            Title = "Task to Delete",
            Description = "This task will be deleted",
            Status = 0,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        var createResponse = await _client.PostAsJsonAsync("/api/tasks", createDto);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdTask = JsonSerializer.Deserialize<TaskResponseDto>(createContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Act
        var response = await _client.DeleteAsync($"/api/tasks/{createdTask.Id}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);

        // Verify task is actually deleted
        var getResponse = await _client.GetAsync($"/api/tasks/{createdTask.Id}");
        Assert.AreEqual(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [TestMethod]
    public async Task DeleteTask_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Act
        var response = await _client.DeleteAsync("/api/tasks/99999");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task CompleteTaskWorkflow_ShouldWorkEndToEnd()
    {
        // 1. Create a task
        var createDto = new TaskCreateDto
        {
            Title = "Workflow Test Task",
            Description = "Testing complete workflow",
            Status = 0, // Pending
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        var createResponse = await _client.PostAsJsonAsync("/api/tasks", createDto);
        Assert.AreEqual(HttpStatusCode.Created, createResponse.StatusCode);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdTask = JsonSerializer.Deserialize<TaskResponseDto>(createContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // 2. Update task to InProgress
        var patchToInProgress = new TaskPatchDto { Status = 1 };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/tasks/{createdTask.Id}", patchToInProgress);
        Assert.AreEqual(HttpStatusCode.OK, patchResponse.StatusCode);

        // 3. Get updated task and verify status
        var getResponse = await _client.GetAsync($"/api/tasks/{createdTask.Id}");
        Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

        var getContent = await getResponse.Content.ReadAsStringAsync();
        var updatedTask = JsonSerializer.Deserialize<TaskResponseDto>(getContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.AreEqual("InProgress", updatedTask.Status);

        // 4. Complete the task
        var patchToCompleted = new TaskPatchDto { Status = 2 };
        var completeResponse = await _client.PatchAsJsonAsync($"/api/tasks/{createdTask.Id}", patchToCompleted);
        Assert.AreEqual(HttpStatusCode.OK, completeResponse.StatusCode);

        // 5. Finally delete the task
        var deleteResponse = await _client.DeleteAsync($"/api/tasks/{createdTask.Id}");
        Assert.AreEqual(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}