using AppointmentSystem.Application.Contracts.Repositories;
using AppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentsByPatient;
using AppointmentSystem.Domain.Entities;
using AppointmentSystem.Domain.Enums;
using FluentAssertions;
using Moq;

namespace AppointmentSystem.Tests.Application.Features.Appointments.Queries;

public class GetAppointmentsByPatientQueryHandlerTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
    private readonly GetAppointmentsByPatientQueryHandler _handler;

    public GetAppointmentsByPatientQueryHandlerTests()
    {
        _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
        _handler = new GetAppointmentsByPatientQueryHandler(_appointmentRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenPatientHasAppointments_ShouldReturnListOfAppointments()
    {
        // Arrange
        var patientId = 1;
        var query = new GetAppointmentsByPatientQuery { PatientId = patientId };

        var appointments = new List<Appointment>
        {
            new Appointment
            {
                Id = 1,
                PatientId = patientId,
                DoctorId = 1,
                AppointmentDate = DateTime.UtcNow.AddDays(1),
                AppointmentTime = TimeSpan.FromHours(10),
                Status = AppointmentStatus.Pending
            },
            new Appointment
            {
                Id = 2,
                PatientId = patientId,
                DoctorId = 2,
                AppointmentDate = DateTime.UtcNow.AddDays(2),
                AppointmentTime = TimeSpan.FromHours(14),
                Status = AppointmentStatus.Confirmed
            }
        };

        _appointmentRepositoryMock
            .Setup(x => x.GetByPatientIdAsync(patientId))
            .ReturnsAsync(appointments);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data.Should().BeEquivalentTo(appointments);
    }

    [Fact]
    public async Task Handle_WhenPatientHasNoAppointments_ShouldReturnEmptyList()
    {
        // Arrange
        var patientId = 1;
        var query = new GetAppointmentsByPatientQuery { PatientId = patientId };

        _appointmentRepositoryMock
            .Setup(x => x.GetByPatientIdAsync(patientId))
            .ReturnsAsync(new List<Appointment>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();
    }
}
