using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Tests.Integration.Controllers;

[TestClass]
public class AuthControllerIntegrationTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    [TestInitialize]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureServices(services =>
                {
                    // Override services for testing if needed
                });
            });

        _client = _factory.CreateClient();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    [TestMethod]
    public async Task Register_ShouldReturnCreated_WhenValidDataProvided()
    {
        // Arrange
        var registerDto = new UserRegisterDto
        {
            Username = "integrationtestuser",
            Email = "integration@test.com",
            Password = "TestPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var userProfile = JsonSerializer.Deserialize<UserProfileDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(userProfile);
        Assert.AreEqual("integrationtestuser", userProfile.Username);
        Assert.AreEqual("integration@test.com", userProfile.Email);
    }

    [TestMethod]
    public async Task Register_ShouldReturnBadRequest_WhenDuplicateUsername()
    {
        // Arrange
        var registerDto = new UserRegisterDto
        {
            Username = "demo_user", // Already exists from seeded data
            Email = "newemail@test.com",
            Password = "TestPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.IsTrue(responseContent.Contains("Username already exists"));
    }

    [TestMethod]
    public async Task Login_ShouldReturnOkWithToken_WhenValidCredentials()
    {
        // Arrange
        var loginDto = new UserLoginDto
        {
            Username = "demo_user",
            Password = "Demo123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var loginResponse = JsonSerializer.Deserialize<LoginResponseDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(loginResponse);
        Assert.IsNotNull(loginResponse.Token);
        Assert.IsTrue(loginResponse.Token.Length > 0);
        Assert.IsNotNull(loginResponse.User);
        Assert.AreEqual("demo_user", loginResponse.User.Username);
    }

    [TestMethod]
    public async Task Login_ShouldReturnUnauthorized_WhenInvalidCredentials()
    {
        // Arrange
        var loginDto = new UserLoginDto
        {
            Username = "demo_user",
            Password = "WrongPassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task GetProfile_ShouldReturnUnauthorized_WhenNoToken()
    {
        // Act
        var response = await _client.GetAsync("/api/auth/profile");

        // Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task GetProfile_ShouldReturnOk_WhenValidToken()
    {
        // Arrange - First login to get token
        var loginDto = new UserLoginDto
        {
            Username = "demo_user",
            Password = "Demo123!"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginResult = JsonSerializer.Deserialize<LoginResponseDto>(loginContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Add token to subsequent requests
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);

        // Act
        var response = await _client.GetAsync("/api/auth/profile");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var userProfile = JsonSerializer.Deserialize<UserProfileDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(userProfile);
        Assert.AreEqual("demo_user", userProfile.Username);
    }

    [TestMethod]
    public async Task Register_ShouldReturnBadRequest_WhenInvalidData()
    {
        // Arrange - Invalid password (no uppercase)
        var registerDto = new UserRegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "weakpassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}