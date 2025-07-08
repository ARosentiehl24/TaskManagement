using Microsoft.Extensions.Caching.Memory;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Repositories;

namespace TaskManagement.Tests.Unit.Infrastructure;

[TestClass]
public class UserRepositoryTests
{
    private IMemoryCache _memoryCache;
    private UserRepository _userRepository;

    [TestInitialize]
    public void Setup()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _userRepository = new UserRepository(_memoryCache);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _memoryCache?.Dispose();
    }

    [TestMethod]
    public async System.Threading.Tasks.Task CreateAsync_ShouldCreateUserWithGeneratedId()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _userRepository.CreateAsync(user);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual("testuser", result.Username);
        Assert.AreEqual("test@example.com", result.Email);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task CreateAsync_ShouldIncrementIdForMultipleUsers()
    {
        // Arrange
        var user1 = new User { Username = "user1", Email = "user1@example.com", PasswordHash = "hash1" };
        var user2 = new User { Username = "user2", Email = "user2@example.com", PasswordHash = "hash2" };

        // Act
        var result1 = await _userRepository.CreateAsync(user1);
        var result2 = await _userRepository.CreateAsync(user2);

        // Assert
        Assert.AreEqual(1, result1.Id);
        Assert.AreEqual(2, result2.Id);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@example.com", PasswordHash = "hash" };
        var createdUser = await _userRepository.CreateAsync(user);

        // Act
        var result = await _userRepository.GetByIdAsync(createdUser.Id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(createdUser.Id, result.Id);
        Assert.AreEqual("testuser", result.Username);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Act
        var result = await _userRepository.GetByIdAsync(999);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task GetByUsernameAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@example.com", PasswordHash = "hash" };
        await _userRepository.CreateAsync(user);

        // Act
        var result = await _userRepository.GetByUsernameAsync("testuser");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("testuser", result.Username);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task GetByUsernameAsync_ShouldBeCaseInsensitive()
    {
        // Arrange
        var user = new User { Username = "TestUser", Email = "test@example.com", PasswordHash = "hash" };
        await _userRepository.CreateAsync(user);

        // Act
        var result = await _userRepository.GetByUsernameAsync("testuser");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("TestUser", result.Username);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task GetByEmailAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@example.com", PasswordHash = "hash" };
        await _userRepository.CreateAsync(user);

        // Act
        var result = await _userRepository.GetByEmailAsync("test@example.com");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("test@example.com", result.Email);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task UsernameExistsAsync_ShouldReturnTrue_WhenUsernameExists()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@example.com", PasswordHash = "hash" };
        await _userRepository.CreateAsync(user);

        // Act
        var result = await _userRepository.UsernameExistsAsync("testuser");

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task UsernameExistsAsync_ShouldReturnFalse_WhenUsernameDoesNotExist()
    {
        // Act
        var result = await _userRepository.UsernameExistsAsync("nonexistent");

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task EmailExistsAsync_ShouldReturnTrue_WhenEmailExists()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@example.com", PasswordHash = "hash" };
        await _userRepository.CreateAsync(user);

        // Act
        var result = await _userRepository.EmailExistsAsync("test@example.com");

        // Assert
        Assert.IsTrue(result);
    }
}
