using AppointmentSystem.Domain.Common;
using AppointmentSystem.Domain.Entities;
using MediatR;

namespace AppointmentSystem.Application.Features.Doctors.Queries.GetAllDoctors;

public class GetAllDoctorsQuery : IRequest<Result<IEnumerable<Doctor>>>
{
}
