using AppointmentSystem.Application.Contracts.Repositories;
using AppointmentSystem.Domain.Common;
using AppointmentSystem.Domain.Entities;
using MediatR;

namespace AppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentsByPatient;

public class GetAppointmentsByPatientQueryHandler : IRequestHandler<GetAppointmentsByPatientQuery, Result<IEnumerable<Appointment>>>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public GetAppointmentsByPatientQueryHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Result<IEnumerable<Appointment>>> Handle(GetAppointmentsByPatientQuery request, CancellationToken cancellationToken)
    {
        var appointments = await _appointmentRepository.GetByPatientIdAsync(request.PatientId);
        return Result<IEnumerable<Appointment>>.Success(appointments);
    }
}
