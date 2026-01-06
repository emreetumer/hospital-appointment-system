using AppointmentSystem.Domain.Entities;

namespace AppointmentSystem.Application.Contracts.Repositories;

public interface IDoctorRepository
{
    Task<Doctor> GetByIdAsync(int id);
    Task<Doctor> GetByUserIdAsync(int userId);
    Task<IEnumerable<Doctor>> GetAllAsync();
    Task<IEnumerable<Doctor>> GetByDepartmentIdAsync(int departmentId);
    Task<int> CreateAsync(Doctor doctor);
    Task<bool> UpdateAsync(Doctor doctor);
    Task<bool> DeleteAsync(int id);
}
