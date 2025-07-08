using Microsoft.Extensions.Caching.Memory;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Data;

/// <summary>
/// Seeds initial data for demonstration purposes
/// </summary>
public class DataSeeder(IMemoryCache memoryCache, IPasswordHasher passwordHasher)
{
    /// <summary>
    /// Seeds demo users and tasks
    /// </summary>
    public void SeedData()
    {
        SeedUsers();
        SeedTasks();
    }

    /// <summary>
    /// Seeds demo users
    /// </summary>
    private void SeedUsers()
    {
        var users = new List<User>
            {
                new() {
                    Id = 1,
                    Username = "demo_user",
                    Email = "demo@example.com",
                    PasswordHash = passwordHasher.HashPassword("Demo123!"),
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                },
                new() {
                    Id = 2,
                    Username = "john_doe",
                    Email = "john.doe@example.com",
                    PasswordHash = passwordHasher.HashPassword("John123!"),
                    CreatedAt = DateTime.UtcNow.AddDays(-15)
                }
            };

        memoryCache.Set("Users", users);
        memoryCache.Set("UserIdCounter", 2);
    }

    /// <summary>
    /// Seeds demo tasks
    /// </summary>
    private void SeedTasks()
    {
        var tasks = new List<Domain.Entities.Task>
            {
                new() {
                    Id = 1,
                    Title = "Complete API Documentation",
                    Description = "Write comprehensive API documentation for the task management system",
                    Status = Domain.Enums.TaskStatus.InProgress,
                    DueDate = DateTime.UtcNow.AddDays(7),
                    UserId = 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new() {
                    Id = 2,
                    Title = "Implement User Authentication",
                    Description = "Add JWT-based authentication to secure the API endpoints",
                    Status =  Domain.Enums.TaskStatus.Completed,
                    DueDate = DateTime.UtcNow.AddDays(-1),
                    UserId = 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new() {
                    Id = 3,
                    Title = "Setup CI/CD Pipeline",
                    Description = "Configure automated deployment pipeline for the application",
                    Status =  Domain.Enums.TaskStatus.Pending,
                    DueDate = DateTime.UtcNow.AddDays(14),
                    UserId = 2,
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                }
            };

        memoryCache.Set("Tasks", tasks);
        memoryCache.Set("TaskIdCounter", 3);
    }
}
