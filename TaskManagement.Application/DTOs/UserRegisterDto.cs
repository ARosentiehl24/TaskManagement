namespace TaskManagement.Application.DTOs;

/// <summary>
/// DTO for user registration
/// </summary>
public class UserRegisterDto
{
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}