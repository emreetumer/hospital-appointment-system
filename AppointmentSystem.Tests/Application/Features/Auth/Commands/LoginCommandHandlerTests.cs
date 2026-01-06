using AppointmentSystem.Application.Contracts.Repositories;
using AppointmentSystem.Application.Contracts.Services;
using AppointmentSystem.Application.Features.Auth.Commands.Login;
using AppointmentSystem.Domain.Entities;
using AppointmentSystem.Domain.Enums;
using FluentAssertions;
using Moq;

namespace AppointmentSystem.Tests.Application.Features.Auth.Commands;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _authServiceMock = new Mock<IAuthService>();
        _handler = new LoginCommandHandler(_userRepositoryMock.Object, _authServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "nonexistent@test.com",
            Password = "Test123!"
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync((User)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Invalid email or password");
        _authServiceMock.Verify(x => x.ValidatePasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserIsInactive_ShouldReturnFailure()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@test.com",
            Password = "Test123!"
        };

        var user = new User
        {
            Id = 1,
            Email = command.Email,
            PasswordHash = "hashed_password",
            IsActive = false,
            Role = UserRoles.Patient
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("User account is inactive");
        _authServiceMock.Verify(x => x.ValidatePasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenPasswordIsInvalid_ShouldReturnFailure()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@test.com",
            Password = "WrongPassword"
        };

        var user = new User
        {
            Id = 1,
            Email = command.Email,
            PasswordHash = "hashed_password",
            IsActive = true,
            Role = UserRoles.Patient
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync(user);

        _authServiceMock
            .Setup(x => x.ValidatePasswordAsync(command.Password, user.PasswordHash))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Invalid email or password");
        _authServiceMock.Verify(x => x.GenerateJwtTokenAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCredentialsAreValid_ShouldReturnSuccessWithToken()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@test.com",
            Password = "Test123!"
        };

        var user = new User
        {
            Id = 1,
            Email = command.Email,
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hashed_password",
            IsActive = true,
            Role = UserRoles.Patient
        };

        var expectedToken = "jwt_token_here";

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync(user);

        _authServiceMock
            .Setup(x => x.ValidatePasswordAsync(command.Password, user.PasswordHash))
            .ReturnsAsync(true);

        _authServiceMock
            .Setup(x => x.GenerateJwtTokenAsync(user.Id, user.Email, user.Role))
            .ReturnsAsync(expectedToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Login successful");
        result.Data.Should().NotBeNull();
        result.Data.UserId.Should().Be(user.Id);
        result.Data.Email.Should().Be(user.Email);
        result.Data.FullName.Should().Be("John Doe");
        result.Data.Role.Should().Be(user.Role);
        result.Data.Token.Should().Be(expectedToken);
    }
}
