using AppointmentSystem.Domain.Common;
using MediatR;

namespace AppointmentSystem.Application.Features.Appointments.Commands.CreateAppointment;

public class CreateAppointmentCommand : IRequest<Result<int>>
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan AppointmentTime { get; set; }
    public string Notes { get; set; }
}
