using AppointmentSystem.Application.Contracts.Repositories;
using AppointmentSystem.Application.Features.Departments.Queries.GetAllDepartments;
using AppointmentSystem.Domain.Entities;
using FluentAssertions;
using Moq;

namespace AppointmentSystem.Tests.Application.Features.Departments.Queries;

public class GetAllDepartmentsQueryHandlerTests
{
    private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
    private readonly GetAllDepartmentsQueryHandler _handler;

    public GetAllDepartmentsQueryHandlerTests()
    {
        _departmentRepositoryMock = new Mock<IDepartmentRepository>();
        _handler = new GetAllDepartmentsQueryHandler(_departmentRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenActiveDepartmentsExist_ShouldReturnListOfDepartments()
    {
        // Arrange
        var query = new GetAllDepartmentsQuery();

        var departments = new List<Department>
        {
            new Department
            {
                Id = 1,
                Name = "Cardiology",
                Description = "Heart and cardiovascular system",
                IsActive = true
            },
            new Department
            {
                Id = 2,
                Name = "Neurology",
                Description = "Brain and nervous system",
                IsActive = true
            }
        };

        _departmentRepositoryMock
            .Setup(x => x.GetActiveAsync())
            .ReturnsAsync(departments);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data.Should().BeEquivalentTo(departments);
    }

    [Fact]
    public async Task Handle_WhenNoActiveDepartmentsExist_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetAllDepartmentsQuery();

        _departmentRepositoryMock
            .Setup(x => x.GetActiveAsync())
            .ReturnsAsync(new List<Department>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();
    }
}
