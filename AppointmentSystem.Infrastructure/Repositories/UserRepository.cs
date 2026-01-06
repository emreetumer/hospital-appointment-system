using AppointmentSystem.Application.Contracts.Repositories;
using AppointmentSystem.Domain.Entities;
using AppointmentSystem.Infrastructure.Data;
using Dapper;

namespace AppointmentSystem.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DapperContext _context;

    public UserRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<User> GetByIdAsync(int id)
    {
        var query = "SELECT * FROM Users WHERE Id = @Id";
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(query, new { Id = id });
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        var query = "SELECT * FROM Users WHERE Email = @Email";
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(query, new { Email = email });
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var query = "SELECT * FROM Users ORDER BY CreatedAt DESC";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<User>(query);
    }

    public async Task<int> CreateAsync(User user)
    {
        var query = @"
            INSERT INTO Users (Email, PasswordHash, FirstName, LastName, PhoneNumber, Role, IsActive, CreatedAt)
            VALUES (@Email, @PasswordHash, @FirstName, @LastName, @PhoneNumber, @Role, @IsActive, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() as int)";

        using var connection = _context.CreateConnection();
        return await connection.QuerySingleAsync<int>(query, user);
    }

    public async Task<bool> UpdateAsync(User user)
    {
        var query = @"
            UPDATE Users 
            SET Email = @Email, 
                FirstName = @FirstName, 
                LastName = @LastName, 
                PhoneNumber = @PhoneNumber, 
                IsActive = @IsActive,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        using var connection = _context.CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(query, user);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var query = "DELETE FROM Users WHERE Id = @Id";
        using var connection = _context.CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<bool> ExistsAsync(string email)
    {
        var query = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
        using var connection = _context.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(query, new { Email = email });
        return count > 0;
    }
}
