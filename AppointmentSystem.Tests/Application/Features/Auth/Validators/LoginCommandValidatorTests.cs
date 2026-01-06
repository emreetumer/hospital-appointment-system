using AppointmentSystem.Application.Features.Auth.Commands.Login;
using FluentAssertions;

namespace AppointmentSystem.Tests.Application.Features.Auth.Validators;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator;

    public LoginCommandValidatorTests()
    {
        _validator = new LoginCommandValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@test.com",
            Password = "Test123!"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WithEmptyEmail_ShouldFailValidation(string email)
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = email,
            Password = "Test123!"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && e.ErrorMessage == "Email is required");
    }

    [Theory]
    [InlineData("invalidemail")]
    [InlineData("@test.com")]
    [InlineData("test@")]
    [InlineData("test.test.com")]
    public void Validate_WithInvalidEmailFormat_ShouldFailValidation(string email)
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = email,
            Password = "Test123!"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && e.ErrorMessage == "Invalid email format");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WithEmptyPassword_ShouldFailValidation(string password)
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@test.com",
            Password = password
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password" && e.ErrorMessage == "Password is required");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WithEmptyEmailAndPassword_ShouldFailValidationForBoth(string value)
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = value,
            Password = value
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Validate_WithValidEmailAndValidPassword_ShouldPassValidation()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "john.doe@example.com",
            Password = "VerySecurePassword123!"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
