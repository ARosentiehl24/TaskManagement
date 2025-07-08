namespace TaskManagement.Application.DTOs;

/// <summary>
/// DTO for login response containing JWT token
/// </summary>
public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;

    public DateTime Expires { get; set; }

    public UserProfileDto User { get; set; } = new();
}
