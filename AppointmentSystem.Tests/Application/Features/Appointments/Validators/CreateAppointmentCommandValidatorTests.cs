using AppointmentSystem.Application.Features.Appointments.Commands.CreateAppointment;
using FluentAssertions;

namespace AppointmentSystem.Tests.Application.Features.Appointments.Validators;

public class CreateAppointmentCommandValidatorTests
{
    private readonly CreateAppointmentCommandValidator _validator;

    public CreateAppointmentCommandValidatorTests()
    {
        _validator = new CreateAppointmentCommandValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var command = new CreateAppointmentCommand
        {
            PatientId = 1,
            DoctorId = 1,
            AppointmentDate = DateTime.Today.AddDays(1),
            AppointmentTime = TimeSpan.FromHours(10),
            Notes = "Regular checkup"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithInvalidPatientId_ShouldFailValidation(int patientId)
    {
        // Arrange
        var command = CreateValidCommand();
        command.PatientId = patientId;

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PatientId" && e.ErrorMessage == "Patient ID is required");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithInvalidDoctorId_ShouldFailValidation(int doctorId)
    {
        // Arrange
        var command = CreateValidCommand();
        command.DoctorId = doctorId;

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DoctorId" && e.ErrorMessage == "Doctor ID is required");
    }

    [Fact]
    public void Validate_WithPastDate_ShouldFailValidation()
    {
        // Arrange
        var command = CreateValidCommand();
        command.AppointmentDate = DateTime.Today.AddDays(-1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AppointmentDate" && e.ErrorMessage == "Appointment date must be today or in the future");
    }

    [Fact]
    public void Validate_WithTodayDate_ShouldPassValidation()
    {
        // Arrange
        var command = CreateValidCommand();
        command.AppointmentDate = DateTime.Today;

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithFutureDate_ShouldPassValidation()
    {
        // Arrange
        var command = CreateValidCommand();
        command.AppointmentDate = DateTime.Today.AddDays(7);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyAppointmentTime_ShouldFailValidation()
    {
        // Arrange
        var command = CreateValidCommand();
        command.AppointmentTime = TimeSpan.Zero;

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AppointmentTime" && e.ErrorMessage == "Appointment time is required");
    }

    [Fact]
    public void Validate_WithNullNotes_ShouldPassValidation()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Notes = null;

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    private CreateAppointmentCommand CreateValidCommand()
    {
        return new CreateAppointmentCommand
        {
            PatientId = 1,
            DoctorId = 1,
            AppointmentDate = DateTime.Today.AddDays(1),
            AppointmentTime = TimeSpan.FromHours(10),
            Notes = "Regular checkup"
        };
    }
}
