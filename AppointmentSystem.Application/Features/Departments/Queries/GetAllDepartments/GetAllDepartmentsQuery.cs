using AppointmentSystem.Domain.Common;
using AppointmentSystem.Domain.Entities;
using MediatR;

namespace AppointmentSystem.Application.Features.Departments.Queries.GetAllDepartments;

public class GetAllDepartmentsQuery : IRequest<Result<IEnumerable<Department>>>
{
}
