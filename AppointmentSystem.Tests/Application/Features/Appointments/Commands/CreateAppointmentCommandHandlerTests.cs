using AppointmentSystem.Application.Contracts.Repositories;
using AppointmentSystem.Application.Features.Appointments.Commands.CreateAppointment;
using AppointmentSystem.Domain.Entities;
using AppointmentSystem.Domain.Enums;
using FluentAssertions;
using Moq;

namespace AppointmentSystem.Tests.Application.Features.Appointments.Commands;

public class CreateAppointmentCommandHandlerTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
    private readonly CreateAppointmentCommandHandler _handler;

    public CreateAppointmentCommandHandlerTests()
    {
        _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _handler = new CreateAppointmentCommandHandler(
            _appointmentRepositoryMock.Object,
            _patientRepositoryMock.Object,
            _doctorRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenPatientDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateAppointmentCommand
        {
            PatientId = 1,
            DoctorId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            AppointmentTime = TimeSpan.FromHours(10),
            Notes = "Test appointment"
        };

        _patientRepositoryMock
            .Setup(x => x.GetByIdAsync(command.PatientId))
            .ReturnsAsync((Patient)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Patient not found");
    }

    [Fact]
    public async Task Handle_WhenDoctorDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateAppointmentCommand
        {
            PatientId = 1,
            DoctorId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            AppointmentTime = TimeSpan.FromHours(10),
            Notes = "Test appointment"
        };

        _patientRepositoryMock
            .Setup(x => x.GetByIdAsync(command.PatientId))
            .ReturnsAsync(new Patient { Id = 1 });

        _doctorRepositoryMock
            .Setup(x => x.GetByIdAsync(command.DoctorId))
            .ReturnsAsync((Doctor)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Doctor not found or inactive");
    }

    [Fact]
    public async Task Handle_WhenDoctorIsInactive_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateAppointmentCommand
        {
            PatientId = 1,
            DoctorId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            AppointmentTime = TimeSpan.FromHours(10),
            Notes = "Test appointment"
        };

        _patientRepositoryMock
            .Setup(x => x.GetByIdAsync(command.PatientId))
            .ReturnsAsync(new Patient { Id = 1 });

        _doctorRepositoryMock
            .Setup(x => x.GetByIdAsync(command.DoctorId))
            .ReturnsAsync(new Doctor { Id = 1, IsActive = false });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Doctor not found or inactive");
    }

    [Fact]
    public async Task Handle_WhenTimeSlotIsNotAvailable_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateAppointmentCommand
        {
            PatientId = 1,
            DoctorId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            AppointmentTime = TimeSpan.FromHours(10),
            Notes = "Test appointment"
        };

        _patientRepositoryMock
            .Setup(x => x.GetByIdAsync(command.PatientId))
            .ReturnsAsync(new Patient { Id = 1 });

        _doctorRepositoryMock
            .Setup(x => x.GetByIdAsync(command.DoctorId))
            .ReturnsAsync(new Doctor { Id = 1, IsActive = true });

        _appointmentRepositoryMock
            .Setup(x => x.IsTimeSlotAvailableAsync(command.DoctorId, command.AppointmentDate, command.AppointmentTime))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("This time slot is not available");
    }

    [Fact]
    public async Task Handle_WhenAllValidationsPass_ShouldCreateAppointmentSuccessfully()
    {
        // Arrange
        var command = new CreateAppointmentCommand
        {
            PatientId = 1,
            DoctorId = 2,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            AppointmentTime = TimeSpan.FromHours(10),
            Notes = "Test appointment"
        };

        var expectedAppointmentId = 123;

        _patientRepositoryMock
            .Setup(x => x.GetByIdAsync(command.PatientId))
            .ReturnsAsync(new Patient { Id = 1 });

        _doctorRepositoryMock
            .Setup(x => x.GetByIdAsync(command.DoctorId))
            .ReturnsAsync(new Doctor { Id = 2, IsActive = true });

        _appointmentRepositoryMock
            .Setup(x => x.IsTimeSlotAvailableAsync(command.DoctorId, command.AppointmentDate, command.AppointmentTime))
            .ReturnsAsync(true);

        _appointmentRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Appointment>()))
            .ReturnsAsync(expectedAppointmentId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(expectedAppointmentId);
        result.Message.Should().Be("Appointment created successfully");

        _appointmentRepositoryMock.Verify(x => x.CreateAsync(It.Is<Appointment>(a =>
            a.PatientId == command.PatientId &&
            a.DoctorId == command.DoctorId &&
            a.AppointmentDate == command.AppointmentDate &&
            a.AppointmentTime == command.AppointmentTime &&
            a.Status == AppointmentStatus.Pending &&
            a.Notes == command.Notes
        )), Times.Once);
    }
}
