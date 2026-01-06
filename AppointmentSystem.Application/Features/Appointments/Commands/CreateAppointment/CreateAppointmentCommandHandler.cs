using AppointmentSystem.Application.Contracts.Repositories;
using AppointmentSystem.Domain.Common;
using AppointmentSystem.Domain.Entities;
using AppointmentSystem.Domain.Enums;
using MediatR;

namespace AppointmentSystem.Application.Features.Appointments.Commands.CreateAppointment;

public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, Result<int>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;

    public CreateAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IPatientRepository patientRepository,
        IDoctorRepository doctorRepository)
    {
        _appointmentRepository = appointmentRepository;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
    }

    public async Task<Result<int>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAsync(request.PatientId);
        if (patient == null)
        {
            return Result<int>.Failure("Patient not found");
        }

        var doctor = await _doctorRepository.GetByIdAsync(request.DoctorId);
        if (doctor == null || !doctor.IsActive)
        {
            return Result<int>.Failure("Doctor not found or inactive");
        }

        var isAvailable = await _appointmentRepository.IsTimeSlotAvailableAsync(
            request.DoctorId, 
            request.AppointmentDate, 
            request.AppointmentTime);

        if (!isAvailable)
        {
            return Result<int>.Failure("This time slot is not available");
        }

        var appointment = new Appointment
        {
            PatientId = request.PatientId,
            DoctorId = request.DoctorId,
            AppointmentDate = request.AppointmentDate,
            AppointmentTime = request.AppointmentTime,
            Status = AppointmentStatus.Pending,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        var appointmentId = await _appointmentRepository.CreateAsync(appointment);

        return Result<int>.Success(appointmentId, "Appointment created successfully");
    }
}
