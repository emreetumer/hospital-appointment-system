namespace AppointmentSystem.Application.Contracts.Services;

public interface IAuthService
{
    Task<string> GenerateJwtTokenAsync(int userId, string email, string role);
    Task<bool> ValidatePasswordAsync(string password, string passwordHash);
    Task<string> HashPasswordAsync(string password);
}
