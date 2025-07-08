using Microsoft.Extensions.Caching.Memory;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of user repository
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly IMemoryCache _memoryCache;
    private const string UsersKey = "Users";
    private const string UserIdCounterKey = "UserIdCounter";

    public UserRepository(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        InitializeUsers();
    }

    /// <summary>
    /// Gets user by ID from memory cache
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User if found</returns>
    public Task<User?> GetByIdAsync(int id)
    {
        var users = GetUsers();
        var user = users.FirstOrDefault(u => u.Id == id);
        return System.Threading.Tasks.Task.FromResult(user);
    }

    /// <summary>
    /// Gets user by username from memory cache
    /// </summary>
    /// <param name="username">Username</param>
    /// <returns>User if found</returns>
    public Task<User?> GetByUsernameAsync(string username)
    {
        var users = GetUsers();
        var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        return System.Threading.Tasks.Task.FromResult(user);
    }

    /// <summary>
    /// Gets user by email from memory cache
    /// </summary>
    /// <param name="email">Email address</param>
    /// <returns>User if found</returns>
    public Task<User?> GetByEmailAsync(string email)
    {
        var users = GetUsers();
        var user = users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        return System.Threading.Tasks.Task.FromResult(user);
    }

    /// <summary>
    /// Creates a new user in memory cache
    /// </summary>
    /// <param name="user">User to create</param>
    /// <returns>Created user with assigned ID</returns>
    public Task<User> CreateAsync(User user)
    {
        var users = GetUsers();
        var nextId = GetNextUserId();

        user.Id = nextId;
        users.Add(user);

        _memoryCache.Set(UsersKey, users);
        _memoryCache.Set(UserIdCounterKey, nextId);

        return System.Threading.Tasks.Task.FromResult(user);
    }

    /// <summary>
    /// Checks if username exists in memory cache
    /// </summary>
    /// <param name="username">Username to check</param>
    /// <returns>True if username exists</returns>
    public Task<bool> UsernameExistsAsync(string username)
    {
        var users = GetUsers();
        var exists = users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        return System.Threading.Tasks.Task.FromResult(exists);
    }

    /// <summary>
    /// Checks if email exists in memory cache
    /// </summary>
    /// <param name="email">Email to check</param>
    /// <returns>True if email exists</returns>
    public Task<bool> EmailExistsAsync(string email)
    {
        var users = GetUsers();
        var exists = users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        return System.Threading.Tasks.Task.FromResult(exists);
    }

    /// <summary>
    /// Gets all users from memory cache or initializes empty list
    /// </summary>
    /// <returns>List of users</returns>
    private List<User> GetUsers()
    {
        return _memoryCache.GetOrCreate(UsersKey, factory => new List<User>()) ?? new List<User>();
    }

    /// <summary>
    /// Gets and increments user ID counter
    /// </summary>
    /// <returns>Next available user ID</returns>
    private int GetNextUserId()
    {
        var currentId = _memoryCache.GetOrCreate(UserIdCounterKey, factory => 0);
        return currentId + 1;
    }

    /// <summary>
    /// Initializes demo users in memory cache
    /// </summary>
    private void InitializeUsers()
    {
        if (_memoryCache.TryGetValue(UsersKey, out _))
            return; // Already initialized

        var users = new List<User>();
        _memoryCache.Set(UsersKey, users);
        _memoryCache.Set(UserIdCounterKey, 0);
    }
}