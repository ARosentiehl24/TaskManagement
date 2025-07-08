using Microsoft.Extensions.Configuration;
using Moq;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Tests.Unit.Application;

[TestClass]
public class AuthServiceTests
{
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IPasswordHasher> _mockPasswordHasher;
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<IConfigurationSection> _mockJwtSection;
    private AuthService _authService;

    [TestInitialize]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockJwtSection = new Mock<IConfigurationSection>();

        // Setup JWT configuration
        _mockJwtSection.Setup(x => x["SecretKey"]).Returns("YourVeryLongSecretKeyThatIsAtLeast32CharactersLong!");
        _mockJwtSection.Setup(x => x["Issuer"]).Returns("TaskManagementAPI");
        _mockJwtSection.Setup(x => x["Audience"]).Returns("TaskManagementAPI");
        _mockConfiguration.Setup(x => x.GetSection("JwtSettings")).Returns(_mockJwtSection.Object);

        _authService = new AuthService(_mockUserRepository.Object, _mockPasswordHasher.Object, _mockConfiguration.Object);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task RegisterAsync_ShouldCreateUser_WhenValidDataProvided()
    {
        // Arrange
        var registerDto = new UserRegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "TestPassword123!"
        };

        var hashedPassword = "hashed_password";
        var createdUser = new User
        {
            Id = 1,
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = hashedPassword,
            CreatedAt = DateTime.UtcNow
        };

        _mockUserRepository.Setup(x => x.UsernameExistsAsync(registerDto.Username)).ReturnsAsync(false);
        _mockUserRepository.Setup(x => x.EmailExistsAsync(registerDto.Email)).ReturnsAsync(false);
        _mockPasswordHasher.Setup(x => x.HashPassword(registerDto.Password)).Returns(hashedPassword);
        _mockUserRepository.Setup(x => x.CreateAsync(It.IsAny<User>())).ReturnsAsync(createdUser);

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual("testuser", result.Username);
        Assert.AreEqual("test@example.com", result.Email);

        _mockUserRepository.Verify(x => x.UsernameExistsAsync(registerDto.Username), Times.Once);
        _mockUserRepository.Verify(x => x.EmailExistsAsync(registerDto.Email), Times.Once);
        _mockPasswordHasher.Verify(x => x.HashPassword(registerDto.Password), Times.Once);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task RegisterAsync_ShouldThrowException_WhenUsernameAlreadyExists()
    {
        // Arrange
        var registerDto = new UserRegisterDto
        {
            Username = "existinguser",
            Email = "test@example.com",
            Password = "TestPassword123!"
        };

        _mockUserRepository.Setup(x => x.UsernameExistsAsync(registerDto.Username)).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
            async () => await _authService.RegisterAsync(registerDto));

        Assert.AreEqual("Username already exists.", exception.Message);
        _mockUserRepository.Verify(x => x.UsernameExistsAsync(registerDto.Username), Times.Once);
        _mockUserRepository.Verify(x => x.EmailExistsAsync(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task RegisterAsync_ShouldThrowException_WhenEmailAlreadyExists()
    {
        // Arrange
        var registerDto = new UserRegisterDto
        {
            Username = "testuser",
            Email = "existing@example.com",
            Password = "TestPassword123!"
        };

        _mockUserRepository.Setup(x => x.UsernameExistsAsync(registerDto.Username)).ReturnsAsync(false);
        _mockUserRepository.Setup(x => x.EmailExistsAsync(registerDto.Email)).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
            async () => await _authService.RegisterAsync(registerDto));

        Assert.AreEqual("Email already exists.", exception.Message);
        _mockUserRepository.Verify(x => x.UsernameExistsAsync(registerDto.Username), Times.Once);
        _mockUserRepository.Verify(x => x.EmailExistsAsync(registerDto.Email), Times.Once);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var loginDto = new UserLoginDto
        {
            Username = "testuser",
            Password = "TestPassword123!"
        };

        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashed_password",
            CreatedAt = DateTime.UtcNow
        };

        _mockUserRepository.Setup(x => x.GetByUsernameAsync(loginDto.Username)).ReturnsAsync(user);
        _mockPasswordHasher.Setup(x => x.VerifyPassword(loginDto.Password, user.PasswordHash)).Returns(true);

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Token);
        Assert.IsTrue(result.Token.Length > 0);
        Assert.IsNotNull(result.User);
        Assert.AreEqual(1, result.User.Id);
        Assert.AreEqual("testuser", result.User.Username);

        _mockUserRepository.Verify(x => x.GetByUsernameAsync(loginDto.Username), Times.Once);
        _mockPasswordHasher.Verify(x => x.VerifyPassword(loginDto.Password, user.PasswordHash), Times.Once);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task LoginAsync_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        var loginDto = new UserLoginDto
        {
            Username = "nonexistentuser",
            Password = "TestPassword123!"
        };

        _mockUserRepository.Setup(x => x.GetByUsernameAsync(loginDto.Username)).ReturnsAsync((User)null);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(
            async () => await _authService.LoginAsync(loginDto));

        Assert.AreEqual("Invalid username or password.", exception.Message);
        _mockUserRepository.Verify(x => x.GetByUsernameAsync(loginDto.Username), Times.Once);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task LoginAsync_ShouldThrowException_WhenPasswordIsInvalid()
    {
        // Arrange
        var loginDto = new UserLoginDto
        {
            Username = "testuser",
            Password = "WrongPassword"
        };

        var user = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = "hashed_password"
        };

        _mockUserRepository.Setup(x => x.GetByUsernameAsync(loginDto.Username)).ReturnsAsync(user);
        _mockPasswordHasher.Setup(x => x.VerifyPassword(loginDto.Password, user.PasswordHash)).Returns(false);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(
            async () => await _authService.LoginAsync(loginDto));

        Assert.AreEqual("Invalid username or password.", exception.Message);
        _mockUserRepository.Verify(x => x.GetByUsernameAsync(loginDto.Username), Times.Once);
        _mockPasswordHasher.Verify(x => x.VerifyPassword(loginDto.Password, user.PasswordHash), Times.Once);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task GetProfileAsync_ShouldReturnProfile_WhenUserExists()
    {
        // Arrange
        var userId = 1;
        var user = new User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@example.com",
            CreatedAt = DateTime.UtcNow
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _authService.GetProfileAsync(userId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(userId, result.Id);
        Assert.AreEqual("testuser", result.Username);
        Assert.AreEqual("test@example.com", result.Email);

        _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
    }

    [TestMethod]
    public async System.Threading.Tasks.Task GetProfileAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = 999;
        _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User)null);

        // Act
        var result = await _authService.GetProfileAsync(userId);

        // Assert
        Assert.IsNull(result);
        _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
    }
}