using AppointmentSystem.Application.Contracts.Repositories;
using AppointmentSystem.Application.Contracts.Services;
using AppointmentSystem.Application.Features.Auth.Commands.Register;
using AppointmentSystem.Domain.Entities;
using AppointmentSystem.Domain.Enums;
using FluentAssertions;
using Moq;

namespace AppointmentSystem.Tests.Application.Features.Auth.Commands;

public class RegisterPatientCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly RegisterPatientCommandHandler _handler;

    public RegisterPatientCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _authServiceMock = new Mock<IAuthService>();
        _handler = new RegisterPatientCommandHandler(
            _userRepositoryMock.Object,
            _patientRepositoryMock.Object,
            _authServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ShouldReturnFailure()
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

        _userRepositoryMock
            .Setup(x => x.ExistsAsync(command.Email))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Email already exists");
        _authServiceMock.Verify(x => x.HashPasswordAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenValidData_ShouldCreatePatientSuccessfully()
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

        var expectedUserId = 1;
        var expectedPatientId = 1;
        var passwordHash = "hashed_password";

        _userRepositoryMock
            .Setup(x => x.ExistsAsync(command.Email))
            .ReturnsAsync(false);

        _authServiceMock
            .Setup(x => x.HashPasswordAsync(command.Password))
            .ReturnsAsync(passwordHash);

        _userRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(expectedUserId);

        _patientRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Patient>()))
            .ReturnsAsync(expectedPatientId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(expectedUserId);
        result.Message.Should().Be("Patient registered successfully");

        _userRepositoryMock.Verify(x => x.CreateAsync(It.Is<User>(u =>
            u.Email == command.Email &&
            u.FirstName == command.FirstName &&
            u.LastName == command.LastName &&
            u.PhoneNumber == command.PhoneNumber &&
            u.Role == UserRoles.Patient &&
            u.IsActive == true &&
            u.PasswordHash == passwordHash
        )), Times.Once);

        _patientRepositoryMock.Verify(x => x.CreateAsync(It.Is<Patient>(p =>
            p.UserId == expectedUserId &&
            p.DateOfBirth == command.DateOfBirth &&
            p.Gender == command.Gender &&
            p.Address == command.Address &&
            p.BloodType == command.BloodType
        )), Times.Once);
    }
}
