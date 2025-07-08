using TaskManagement.Application.Interfaces;

namespace TaskManagement.Infrastructure.Security;

/// <summary>
/// BCrypt implementation of password hasher
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    /// <summary>
    /// Hashes a plain text password using BCrypt
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>BCrypt hashed password</returns>
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
    }

    /// <summary>
    /// Verifies a password against a BCrypt hash
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <param name="hash">BCrypt hash</param>
    /// <returns>True if password matches hash</returns>
    public bool VerifyPassword(string password, string hash)
    {
        try
        {
            return string.IsNullOrWhiteSpace(password)
                ? throw new ArgumentNullException(nameof(password), "Password cannot be null or empty.")
                : BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
