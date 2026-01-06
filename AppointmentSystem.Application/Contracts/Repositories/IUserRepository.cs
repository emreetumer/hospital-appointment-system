using AppointmentSystem.Domain.Entities;

namespace AppointmentSystem.Application.Contracts.Repositories;

public interface IUserRepository
{
    Task<User> GetByIdAsync(int id);
    Task<User> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<int> CreateAsync(User user);
    Task<bool> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(string email);
}
