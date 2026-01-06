using AppointmentSystem.Domain.Entities;

namespace AppointmentSystem.Application.Contracts.Repositories;

public interface IDepartmentRepository
{
    Task<Department> GetByIdAsync(int id);
    Task<IEnumerable<Department>> GetAllAsync();
    Task<IEnumerable<Department>> GetActiveAsync();
    Task<int> CreateAsync(Department department);
    Task<bool> UpdateAsync(Department department);
    Task<bool> DeleteAsync(int id);
}
