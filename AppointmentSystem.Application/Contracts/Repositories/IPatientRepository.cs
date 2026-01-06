using AppointmentSystem.Domain.Entities;

namespace AppointmentSystem.Application.Contracts.Repositories;

public interface IPatientRepository
{
    Task<Patient> GetByIdAsync(int id);
    Task<Patient> GetByUserIdAsync(int userId);
    Task<IEnumerable<Patient>> GetAllAsync();
    Task<int> CreateAsync(Patient patient);
    Task<bool> UpdateAsync(Patient patient);
    Task<bool> DeleteAsync(int id);
}
