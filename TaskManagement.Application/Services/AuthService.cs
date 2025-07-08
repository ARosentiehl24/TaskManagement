using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Services;

/// <summary>
/// Service for handling authentication operations
/// </summary>
public class AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IConfiguration configuration) : IAuthService
{
    /// <summary>
    /// Registers a new user in the system
    /// </summary>
    /// <param name="registerDto">User registration data</param>
    /// <returns>User profile if registration successful</returns>
    /// <exception cref="InvalidOperationException">Thrown when username or email already exists</exception>
    public async Task<UserProfileDto> RegisterAsync(UserRegisterDto registerDto)
    {
        // Check if username already exists
        if (await userRepository.UsernameExistsAsync(registerDto.Username))
        {
            throw new InvalidOperationException("Username already exists.");
        }

        // Check if email already exists
        if (await userRepository.EmailExistsAsync(registerDto.Email))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        // Create new user
        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = passwordHasher.HashPassword(registerDto.Password),
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await userRepository.CreateAsync(user);

        return new UserProfileDto
        {
            Id = createdUser.Id,
            Username = createdUser.Username,
            Email = createdUser.Email,
            CreatedAt = createdUser.CreatedAt
        };
    }

    /// <summary>
    /// Authenticates a user and generates JWT token
    /// </summary>
    /// <param name="loginDto">Login credentials</param>
    /// <returns>Login response with JWT token</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when credentials are invalid</exception>
    public async Task<LoginResponseDto> LoginAsync(UserLoginDto loginDto)
    {
        var user = await userRepository.GetByUsernameAsync(loginDto.Username);
        if (user == null || !passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        var token = GenerateJwtToken(user);
        var expires = DateTime.UtcNow.AddHours(24);

        return new LoginResponseDto
        {
            Token = token,
            Expires = expires,
            User = new UserProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            }
        };
    }

    /// <summary>
    /// Gets user profile by ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User profile if found</returns>
    public async Task<UserProfileDto?> GetProfileAsync(int userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        return user == null
            ? null
            : new UserProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };
    }

    /// <summary>
    /// Generates JWT token for authenticated user
    /// </summary>
    /// <param name="user">User to generate token for</param>
    /// <returns>JWT token string</returns>
    private string GenerateJwtToken(User user)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var issuer = jwtSettings["Issuer"] ?? "TaskManagementAPI";
        var audience = jwtSettings["Audience"] ?? "TaskManagementAPI";

        var key = Encoding.ASCII.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
            Expires = DateTime.UtcNow.AddHours(24),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
