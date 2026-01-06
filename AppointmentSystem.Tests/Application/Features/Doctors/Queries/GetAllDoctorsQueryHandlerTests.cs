using AppointmentSystem.Application.Contracts.Repositories;
using AppointmentSystem.Application.Features.Doctors.Queries.GetAllDoctors;
using AppointmentSystem.Domain.Entities;
using FluentAssertions;
using Moq;

namespace AppointmentSystem.Tests.Application.Features.Doctors.Queries;

public class GetAllDoctorsQueryHandlerTests
{
    private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
    private readonly GetAllDoctorsQueryHandler _handler;

    public GetAllDoctorsQueryHandlerTests()
    {
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _handler = new GetAllDoctorsQueryHandler(_doctorRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenDoctorsExist_ShouldReturnListOfDoctors()
    {
        // Arrange
        var query = new GetAllDoctorsQuery();

        var doctors = new List<Doctor>
        {
            new Doctor
            {
                Id = 1,
                UserId = 1,
                DepartmentId = 1,
                Title = "Cardiologist",
                LicenseNumber = "LIC001",
                IsActive = true
            },
            new Doctor
            {
                Id = 2,
                UserId = 2,
                DepartmentId = 2,
                Title = "Neurologist",
                LicenseNumber = "LIC002",
                IsActive = true
            }
        };

        _doctorRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(doctors);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data.Should().BeEquivalentTo(doctors);
    }

    [Fact]
    public async Task Handle_WhenNoDoctorsExist_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetAllDoctorsQuery();

        _doctorRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Doctor>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();
    }
}
