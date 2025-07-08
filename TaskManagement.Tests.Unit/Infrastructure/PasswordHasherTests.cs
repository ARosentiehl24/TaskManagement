using TaskManagement.Infrastructure.Security;

namespace TaskManagement.Tests.Unit.Infrastructure;

[TestClass]
public class PasswordHasherTests
{
    private PasswordHasher _passwordHasher;

    [TestInitialize]
    public void Setup()
    {
        _passwordHasher = new PasswordHasher();
    }

    [TestMethod]
    public void HashPassword_ShouldReturnHashedPassword()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var hash = _passwordHasher.HashPassword(password);

        // Assert
        Assert.IsNotNull(hash);
        Assert.AreNotEqual(password, hash);
        Assert.IsTrue(hash.Length > 0);
    }

    [TestMethod]
    public void HashPassword_ShouldReturnDifferentHashesForSamePassword()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var hash1 = _passwordHasher.HashPassword(password);
        var hash2 = _passwordHasher.HashPassword(password);

        // Assert
        Assert.AreNotEqual(hash1, hash2);
    }

    [TestMethod]
    public void VerifyPassword_ShouldReturnTrue_WhenPasswordMatches()
    {
        // Arrange
        var password = "TestPassword123!";
        var hash = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(password, hash);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void VerifyPassword_ShouldReturnFalse_WhenPasswordDoesNotMatch()
    {
        // Arrange
        var password = "TestPassword123!";
        var wrongPassword = "WrongPassword123!";
        var hash = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(wrongPassword, hash);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void VerifyPassword_ShouldReturnFalse_WhenHashIsInvalid()
    {
        // Arrange
        var password = "TestPassword123!";
        var invalidHash = "invalidhash";

        // Act
        var result = _passwordHasher.VerifyPassword(password, invalidHash);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void HashPassword_ShouldThrowException_WhenPasswordIsNull()
    {
        // Act
        _passwordHasher.HashPassword(null);
    }
}