using AppointmentSystem.Domain.Common;
using FluentAssertions;

namespace AppointmentSystem.Tests.Domain.Common;

public class ResultTests
{
    [Fact]
    public void Success_WithData_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var data = 123;
        var message = "Operation completed";

        // Act
        var result = Result<int>.Success(data, message);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(data);
        result.Message.Should().Be(message);
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Success_WithoutCustomMessage_ShouldUseDefaultMessage()
    {
        // Arrange
        var data = "test data";

        // Act
        var result = Result<string>.Success(data);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(data);
        result.Message.Should().Be("Operation successful");
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Failure_WithMessage_ShouldCreateFailedResult()
    {
        // Arrange
        var message = "Operation failed";

        // Act
        var result = Result<int>.Failure(message);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().Be(default(int));
        result.Message.Should().Be(message);
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Failure_WithMessageAndErrors_ShouldCreateFailedResultWithErrors()
    {
        // Arrange
        var message = "Validation failed";
        var errors = new List<string> { "Error 1", "Error 2" };

        // Act
        var result = Result<int>.Failure(message, errors);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().Be(default(int));
        result.Message.Should().Be(message);
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void Failure_WithErrorsOnly_ShouldUseDefaultMessage()
    {
        // Arrange
        var errors = new List<string> { "Error 1", "Error 2", "Error 3" };

        // Act
        var result = Result<string>.Failure(errors);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Operation failed");
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void NonGenericResult_Success_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var message = "Operation completed successfully";

        // Act
        var result = Result.Success(message);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(message);
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void NonGenericResult_Success_WithoutMessage_ShouldUseDefaultMessage()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Operation successful");
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void NonGenericResult_Failure_WithMessage_ShouldCreateFailedResult()
    {
        // Arrange
        var message = "Operation failed";

        // Act
        var result = Result.Failure(message);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(message);
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void NonGenericResult_Failure_WithErrorsList_ShouldCreateFailedResultWithErrors()
    {
        // Arrange
        var errors = new List<string> { "Error 1", "Error 2" };

        // Act
        var result = Result.Failure(errors);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Operation failed");
        result.Errors.Should().BeEquivalentTo(errors);
    }
}
