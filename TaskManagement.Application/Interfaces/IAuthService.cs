using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

/// <summary>
/// Interface for authentication service operations
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user in the system
    /// </summary>
    /// <param name="registerDto">User registration data</param>
    /// <returns>User profile if registration successful</returns>
    /// <exception cref="InvalidOperationException">Thrown when username or email already exists</exception>
    Task<UserProfileDto> RegisterAsync(UserRegisterDto registerDto);

    /// <summary>
    /// Authenticates a user and generates JWT token
    /// </summary>
    /// <param name="loginDto">Login credentials</param>
    /// <returns>Login response with JWT token</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when credentials are invalid</exception>
    Task<LoginResponseDto> LoginAsync(UserLoginDto loginDto);

    /// <summary>
    /// Gets user profile by ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User profile if found</returns>
    Task<UserProfileDto?> GetProfileAsync(int userId);
}
