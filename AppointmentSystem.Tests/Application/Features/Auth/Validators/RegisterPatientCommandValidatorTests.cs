using AppointmentSystem.Application.Features.Auth.Commands.Register;
using FluentAssertions;

namespace AppointmentSystem.Tests.Application.Features.Auth.Validators;

public class RegisterPatientCommandValidatorTests
{
    private readonly RegisterPatientCommandValidator _validator;

    public RegisterPatientCommandValidatorTests()
    {
        _validator = new RegisterPatientCommandValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var command = new RegisterPatientCommand
        {
            Email = "test@test.com",
            Password = "Test123!",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "1234567890",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "Male",
            Address = "123 Test St",
            BloodType = "A+"
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
        var command = CreateValidCommand();
        command.Email = email;

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
    public void Validate_WithInvalidEmailFormat_ShouldFailValidation(string email)
    {
        // Arrange
        var command = CreateValidCommand();
        command.Email = email;

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && e.ErrorMessage == "Invalid email format");
    }

    [Fact]
    public void Validate_WithEmptyPassword_ShouldFailValidation()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Password = "";

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password" && e.ErrorMessage == "Password is required");
    }

    [Fact]
    public void Validate_WithShortPassword_ShouldFailValidation()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Password = "12345";

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password" && e.ErrorMessage == "Password must be at least 6 characters");
    }

    [Fact]
    public void Validate_WithEmptyFirstName_ShouldFailValidation()
    {
        // Arrange
        var command = CreateValidCommand();
        command.FirstName = "";

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName" && e.ErrorMessage == "First name is required");
    }

    [Fact]
    public void Validate_WithTooLongFirstName_ShouldFailValidation()
    {
        // Arrange
        var command = CreateValidCommand();
        command.FirstName = new string('A', 51);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName" && e.ErrorMessage == "First name cannot exceed 50 characters");
    }

    [Fact]
    public void Validate_WithEmptyLastName_ShouldFailValidation()
    {
        // Arrange
        var command = CreateValidCommand();
        command.LastName = "";

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LastName" && e.ErrorMessage == "Last name is required");
    }

    [Fact]
    public void Validate_WithTooLongLastName_ShouldFailValidation()
    {
        // Arrange
        var command = CreateValidCommand();
        command.LastName = new string('B', 51);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LastName" && e.ErrorMessage == "Last name cannot exceed 50 characters");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("12345678901")]
    [InlineData("abcdefghij")]
    public void Validate_WithInvalidPhoneNumber_ShouldFailValidation(string phoneNumber)
    {
        // Arrange
        var command = CreateValidCommand();
        command.PhoneNumber = phoneNumber;

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PhoneNumber" && e.ErrorMessage == "Phone number must be 10 digits");
    }

    [Fact]
    public void Validate_WithFutureDateOfBirth_ShouldFailValidation()
    {
        // Arrange
        var command = CreateValidCommand();
        command.DateOfBirth = DateTime.Today.AddDays(1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DateOfBirth" && e.ErrorMessage == "Date of birth must be in the past");
    }

    [Theory]
    [InlineData("Male")]
    [InlineData("Female")]
    [InlineData("Other")]
    public void Validate_WithValidGender_ShouldPassValidation(string gender)
    {
        // Arrange
        var command = CreateValidCommand();
        command.Gender = gender;

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("Unknown")]
    [InlineData("M")]
    public void Validate_WithInvalidGender_ShouldFailValidation(string gender)
    {
        // Arrange
        var command = CreateValidCommand();
        command.Gender = gender;

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    private RegisterPatientCommand CreateValidCommand()
    {
        return new RegisterPatientCommand
        {
            Email = "test@test.com",
            Password = "Test123!",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "1234567890",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "Male",
            Address = "123 Test St",
            BloodType = "A+"
        };
    }
}
