using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Server.Controllers;

namespace TaskManagement.Tests.Unit.API.Controllers;

[TestClass]
public class AuthControllerTests
{
    private Mock<IAuthService> _mockAuthService;
    private AuthController _controller;

    [TestInitialize]
    public void Setup()
    {
        _mockAuthService = new Mock<IAuthService>();
        _controller = new AuthController(_mockAuthService.Object);
    }

    [TestMethod]
    public async Task Register_ShouldReturnCreatedResult_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var registerDto = new UserRegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "TestPassword123!"
        };

        var userProfile = new UserProfileDto
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            CreatedAt = DateTime.UtcNow
        };

        _mockAuthService.Setup(x => x.RegisterAsync(registerDto)).ReturnsAsync(userProfile);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
        var createdResult = result as CreatedAtActionResult;
        Assert.AreEqual(userProfile, createdResult.Value);
        Assert.AreEqual(nameof(AuthController.GetProfile), createdResult.ActionName);

        _mockAuthService.Verify(x => x.RegisterAsync(registerDto), Times.Once);
    }

    [TestMethod]
    public async Task Register_ShouldReturnBadRequest_WhenRegistrationFails()
    {
        // Arrange
        var registerDto = new UserRegisterDto
        {
            Username = "existinguser",
            Email = "test@example.com",
            Password = "TestPassword123!"
        };

        var errorMessage = "Username already exists.";
        _mockAuthService.Setup(x => x.RegisterAsync(registerDto))
            .ThrowsAsync(new InvalidOperationException(errorMessage));

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        var badRequestResult = result as BadRequestObjectResult;

        var errorResponse = badRequestResult.Value;
        Assert.IsNotNull(errorResponse);

        // Use reflection to check the anonymous object
        var errorProperty = errorResponse.GetType().GetProperty("error");
        Assert.AreEqual(errorMessage, errorProperty.GetValue(errorResponse));

        _mockAuthService.Verify(x => x.RegisterAsync(registerDto), Times.Once);
    }

    [TestMethod]
    public async Task Login_ShouldReturnOkResult_WhenLoginIsSuccessful()
    {
        // Arrange
        var loginDto = new UserLoginDto
        {
            Username = "testuser",
            Password = "TestPassword123!"
        };

        var loginResponse = new LoginResponseDto
        {
            Token = "jwt-token",
            Expires = DateTime.UtcNow.AddHours(24),
            User = new UserProfileDto
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                CreatedAt = DateTime.UtcNow
            }
        };

        _mockAuthService.Setup(x => x.LoginAsync(loginDto)).ReturnsAsync(loginResponse);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.AreEqual(loginResponse, okResult.Value);

        _mockAuthService.Verify(x => x.LoginAsync(loginDto), Times.Once);
    }

    [TestMethod]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var loginDto = new UserLoginDto
        {
            Username = "testuser",
            Password = "wrongpassword"
        };

        var errorMessage = "Invalid username or password.";
        _mockAuthService.Setup(x => x.LoginAsync(loginDto))
            .ThrowsAsync(new UnauthorizedAccessException(errorMessage));

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        var unauthorizedResult = result as UnauthorizedObjectResult;

        var errorResponse = unauthorizedResult.Value;
        var errorProperty = errorResponse.GetType().GetProperty("error");
        Assert.AreEqual(errorMessage, errorProperty.GetValue(errorResponse));

        _mockAuthService.Verify(x => x.LoginAsync(loginDto), Times.Once);
    }

    [TestMethod]
    public async Task GetProfile_ShouldReturnOkResult_WhenUserIsAuthenticated()
    {
        // Arrange
        var userId = 1;
        var userProfile = new UserProfileDto
        {
            Id = userId,
            Username = "testuser",
            Email = "test@example.com",
            CreatedAt = DateTime.UtcNow
        };

        // Setup user claims
        var claims = new[]
        {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, "testuser")
            };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        _mockAuthService.Setup(x => x.GetProfileAsync(userId)).ReturnsAsync(userProfile);

        // Act
        var result = await _controller.GetProfile();

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.AreEqual(userProfile, okResult.Value);

        _mockAuthService.Verify(x => x.GetProfileAsync(userId), Times.Once);
    }

    [TestMethod]
    public async Task GetProfile_ShouldReturnUnauthorized_WhenUserIdClaimIsMissing()
    {
        // Arrange
        var claims = new[]
        {
                new Claim(ClaimTypes.Name, "testuser") // Missing NameIdentifier claim
            };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        // Act
        var result = await _controller.GetProfile();

        // Assert
        Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        _mockAuthService.Verify(x => x.GetProfileAsync(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public async Task GetProfile_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = 999;
        var claims = new[]
        {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        _mockAuthService.Setup(x => x.GetProfileAsync(userId)).ReturnsAsync((UserProfileDto)null);

        // Act
        var result = await _controller.GetProfile();

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        _mockAuthService.Verify(x => x.GetProfileAsync(userId), Times.Once);
    }
}