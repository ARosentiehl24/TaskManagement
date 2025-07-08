using FluentValidation.TestHelper;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Validators;

namespace TaskManagement.Tests.Unit.Application.Validators;

[TestClass]
public class UserRegisterDtoValidatorTests
{
    private UserRegisterDtoValidator _validator;

    [TestInitialize]
    public void Setup()
    {
        _validator = new UserRegisterDtoValidator();
    }

    [TestMethod]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        // Arrange
        var dto = new UserRegisterDto
        {
            Username = "validuser",
            Email = "valid@example.com",
            Password = "ValidPass123!"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenUsernameIsEmpty()
    {
        // Arrange
        var dto = new UserRegisterDto
        {
            Username = "",
            Email = "valid@example.com",
            Password = "ValidPass123!"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("Username is required.");
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenUsernameIsTooShort()
    {
        // Arrange
        var dto = new UserRegisterDto
        {
            Username = "ab", // Only 2 characters
            Email = "valid@example.com",
            Password = "ValidPass123!"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("Username must be at least 3 characters long.");
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenUsernameContainsInvalidCharacters()
    {
        // Arrange
        var dto = new UserRegisterDto
        {
            Username = "user@name", // Contains @
            Email = "valid@example.com",
            Password = "ValidPass123!"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("Username can only contain letters, numbers, and underscores.");
    }

    [TestMethod]
    public void Validate_ShouldPass_WhenUsernameContainsValidCharacters()
    {
        var validUsernames = new[] { "user123", "User_Name", "test_user_123", "USERNAME" };

        foreach (var username in validUsernames)
        {
            // Arrange
            var dto = new UserRegisterDto
            {
                Username = username,
                Email = "valid@example.com",
                Password = "ValidPass123!"
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Username);
        }
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenEmailIsInvalid()
    {
        // Arrange
        var dto = new UserRegisterDto
        {
            Username = "validuser",
            Email = "invalid-email",
            Password = "ValidPass123!"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email must be in valid format.");
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenPasswordIsTooShort()
    {
        // Arrange
        var dto = new UserRegisterDto
        {
            Username = "validuser",
            Email = "valid@example.com",
            Password = "Short1!" // Only 7 characters
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 8 characters long.");
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenPasswordLacksUppercase()
    {
        // Arrange
        var dto = new UserRegisterDto
        {
            Username = "validuser",
            Email = "valid@example.com",
            Password = "lowercase123!" // No uppercase
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one uppercase letter, one lowercase letter, and one number.");
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenPasswordLacksLowercase()
    {
        // Arrange
        var dto = new UserRegisterDto
        {
            Username = "validuser",
            Email = "valid@example.com",
            Password = "UPPERCASE123!" // No lowercase
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one uppercase letter, one lowercase letter, and one number.");
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenPasswordLacksNumber()
    {
        // Arrange
        var dto = new UserRegisterDto
        {
            Username = "validuser",
            Email = "valid@example.com",
            Password = "ValidPassword!" // No number
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one uppercase letter, one lowercase letter, and one number.");
    }

    [TestMethod]
    public void Validate_ShouldPass_WhenPasswordMeetsAllRequirements()
    {
        var validPasswords = new[]
        {
                "ValidPass123!",
                "MyPassword1",
                "Complex123Password",
                "Test1234"
            };

        foreach (var password in validPasswords)
        {
            // Arrange
            var dto = new UserRegisterDto
            {
                Username = "validuser",
                Email = "valid@example.com",
                Password = password
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Password);
        }
    }
}
