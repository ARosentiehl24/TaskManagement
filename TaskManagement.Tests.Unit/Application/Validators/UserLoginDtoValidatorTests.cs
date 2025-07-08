using FluentValidation.TestHelper;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Validators;

namespace TaskManagement.Tests.Unit.Application.Validators;

[TestClass]
public class UserLoginDtoValidatorTests
{
    private UserLoginDtoValidator _validator;

    [TestInitialize]
    public void Setup()
    {
        _validator = new UserLoginDtoValidator();
    }

    [TestMethod]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        // Arrange
        var dto = new UserLoginDto
        {
            Username = "validuser",
            Password = "validpassword"
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
        var dto = new UserLoginDto
        {
            Username = "",
            Password = "validpassword"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("Username is required.");
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenPasswordIsEmpty()
    {
        // Arrange
        var dto = new UserLoginDto
        {
            Username = "validuser",
            Password = ""
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required.");
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenUsernameIsTooLong()
    {
        // Arrange
        var dto = new UserLoginDto
        {
            Username = new string('a', 51), // 51 characters
            Password = "validpassword"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("Username cannot exceed 50 characters.");
    }

    [TestMethod]
    public void Validate_ShouldPass_WhenUsernameIsAtMaxLength()
    {
        // Arrange
        var dto = new UserLoginDto
        {
            Username = new string('a', 50), // Exactly 50 characters
            Password = "validpassword"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Username);
    }

    [TestMethod]
    public void Validate_ShouldPass_WhenPasswordDoesNotMeetRegistrationRequirements()
    {
        // Note: Login validation is less strict than registration
        // Arrange
        var dto = new UserLoginDto
        {
            Username = "validuser",
            Password = "weak" // Weak password is allowed for login
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }
}