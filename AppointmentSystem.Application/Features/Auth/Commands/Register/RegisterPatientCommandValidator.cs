using FluentValidation;

namespace AppointmentSystem.Application.Features.Auth.Commands.Register;

public class RegisterPatientCommandValidator : AbstractValidator<RegisterPatientCommand>
{
    public RegisterPatientCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^[0-9]{10}$").WithMessage("Phone number must be 10 digits")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required")
            .Must(x => x == "Male" || x == "Female" || x == "Other")
            .WithMessage("Gender must be Male, Female, or Other");
    }
}
