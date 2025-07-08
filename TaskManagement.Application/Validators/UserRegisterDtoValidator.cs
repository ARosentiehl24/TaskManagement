using FluentValidation;
using System.Text.RegularExpressions;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Validators;

/// <summary>
/// Validator for UserRegisterDto
/// </summary>
public class UserRegisterDtoValidator : AbstractValidator<UserRegisterDto>
{
    public UserRegisterDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(50)
            .WithMessage("Username cannot exceed 50 characters.")
            .Matches("^[a-zA-Z0-9_]+$")
            .WithMessage("Username can only contain letters, numbers, and underscores.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email must be in valid format.")
            .MaximumLength(255)
            .WithMessage("Email cannot exceed 255 characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.")
            .Must(ContainRequiredCharacters)
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one number.");
    }

    private static bool ContainRequiredCharacters(string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        var hasUpper = Regex.IsMatch(password, @"[A-Z]");
        var hasLower = Regex.IsMatch(password, @"[a-z]");
        var hasNumber = Regex.IsMatch(password, @"\d");

        return hasUpper && hasLower && hasNumber;
    }
}
