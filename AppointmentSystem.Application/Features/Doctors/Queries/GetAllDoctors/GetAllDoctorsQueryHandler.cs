using AppointmentSystem.Application.Contracts.Repositories;
using AppointmentSystem.Domain.Common;
using AppointmentSystem.Domain.Entities;
using MediatR;

namespace AppointmentSystem.Application.Features.Doctors.Queries.GetAllDoctors;

public class GetAllDoctorsQueryHandler : IRequestHandler<GetAllDoctorsQuery, Result<IEnumerable<Doctor>>>
{
    private readonly IDoctorRepository _doctorRepository;

    public GetAllDoctorsQueryHandler(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task<Result<IEnumerable<Doctor>>> Handle(GetAllDoctorsQuery request, CancellationToken cancellationToken)
    {
        var doctors = await _doctorRepository.GetAllAsync();
        return Result<IEnumerable<Doctor>>.Success(doctors);
    }
}
