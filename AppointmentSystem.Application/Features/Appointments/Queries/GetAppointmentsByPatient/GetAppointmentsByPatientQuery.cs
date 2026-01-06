using AppointmentSystem.Domain.Common;
using AppointmentSystem.Domain.Entities;
using MediatR;

namespace AppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentsByPatient;

public class GetAppointmentsByPatientQuery : IRequest<Result<IEnumerable<Appointment>>>
{
    public int PatientId { get; set; }
}
