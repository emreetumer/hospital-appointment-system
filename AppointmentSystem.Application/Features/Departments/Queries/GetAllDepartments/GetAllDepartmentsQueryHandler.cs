using AppointmentSystem.Application.Contracts.Repositories;
using AppointmentSystem.Domain.Common;
using AppointmentSystem.Domain.Entities;
using MediatR;

namespace AppointmentSystem.Application.Features.Departments.Queries.GetAllDepartments;

public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, Result<IEnumerable<Department>>>
{
    private readonly IDepartmentRepository _departmentRepository;

    public GetAllDepartmentsQueryHandler(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<Result<IEnumerable<Department>>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var departments = await _departmentRepository.GetActiveAsync();
        return Result<IEnumerable<Department>>.Success(departments);
    }
}
