using AppointmentSystem.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;

namespace AppointmentSystem.Tests.Infrastructure.Services;

public class AuthServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        SetupConfiguration();
        _authService = new AuthService(_configurationMock.Object);
    }

    private void SetupConfiguration()
    {
        var jwtSettingsSectionMock = new Mock<IConfigurationSection>();
        
        _configurationMock
            .Setup(x => x.GetSection("JwtSettings"))
            .Returns(jwtSettingsSectionMock.Object);

        jwtSettingsSectionMock
            .Setup(x => x["SecretKey"])
            .Returns("ThisIsAVerySecureSecretKeyForJWTTokenGeneration12345");

        jwtSettingsSectionMock
            .Setup(x => x["Issuer"])
            .Returns("AppointmentSystem");

        jwtSettingsSectionMock
            .Setup(x => x["Audience"])
            .Returns("AppointmentSystemUsers");

        jwtSettingsSectionMock
            .Setup(x => x["ExpiryInMinutes"])
            .Returns("60");
    }

    [Fact]
    public async Task HashPasswordAsync_ShouldReturnHashedPassword()
    {
        // Arrange
        var password = "Test123!";

        // Act
        var hashedPassword = await _authService.HashPasswordAsync(password);

        // Assert
        hashedPassword.Should().NotBeNullOrEmpty();
        hashedPassword.Should().NotBe(password);
        hashedPassword.Length.Should().BeGreaterThan(20);
    }

    [Fact]
    public async Task ValidatePasswordAsync_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "Test123!";
        var hashedPassword = await _authService.HashPasswordAsync(password);

        // Act
        var isValid = await _authService.ValidatePasswordAsync(password, hashedPassword);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidatePasswordAsync_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var correctPassword = "Test123!";
        var incorrectPassword = "WrongPassword";
        var hashedPassword = await _authService.HashPasswordAsync(correctPassword);

        // Act
        var isValid = await _authService.ValidatePasswordAsync(incorrectPassword, hashedPassword);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public async Task GenerateJwtTokenAsync_ShouldReturnValidToken()
    {
        // Arrange
        var userId = 1;
        var email = "test@test.com";
        var role = "Patient";

        // Act
        var token = await _authService.GenerateJwtTokenAsync(userId, email, role);

        // Assert
        token.Should().NotBeNullOrEmpty();

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Subject.Should().Be(userId.ToString());
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == email);
        jwtToken.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == role);
        jwtToken.Issuer.Should().Be("AppointmentSystem");
        jwtToken.Audiences.Should().Contain("AppointmentSystemUsers");
    }

    [Fact]
    public async Task GenerateJwtTokenAsync_ShouldIncludeAllRequiredClaims()
    {
        // Arrange
        var userId = 123;
        var email = "john.doe@test.com";
        var role = "Doctor";

        // Act
        var token = await _authService.GenerateJwtTokenAsync(userId, email, role);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == email);
        jwtToken.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == role);
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
    }

    [Fact]
    public async Task HashPasswordAsync_WithSamePassword_ShouldGenerateDifferentHashes()
    {
        // Arrange
        var password = "Test123!";

        // Act
        var hash1 = await _authService.HashPasswordAsync(password);
        var hash2 = await _authService.HashPasswordAsync(password);

        // Assert
        hash1.Should().NotBe(hash2);
        
        // Both should still validate correctly
        var isValid1 = await _authService.ValidatePasswordAsync(password, hash1);
        var isValid2 = await _authService.ValidatePasswordAsync(password, hash2);
        
        isValid1.Should().BeTrue();
        isValid2.Should().BeTrue();
    }
}
