using FluentValidation;

namespace AppointmentSystem.Application.Features.Appointments.Commands.CreateAppointment;

public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .GreaterThan(0).WithMessage("Patient ID is required");

        RuleFor(x => x.DoctorId)
            .GreaterThan(0).WithMessage("Doctor ID is required");

        RuleFor(x => x.AppointmentDate)
            .NotEmpty().WithMessage("Appointment date is required")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Appointment date must be today or in the future");

        RuleFor(x => x.AppointmentTime)
            .NotEmpty().WithMessage("Appointment time is required");
    }
}
