using AppointmentSystem.Application.Contracts.Repositories;
using AppointmentSystem.Domain.Entities;
using AppointmentSystem.Infrastructure.Data;
using Dapper;

namespace AppointmentSystem.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly DapperContext _context;

    public DepartmentRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Department> GetByIdAsync(int id)
    {
        var query = "SELECT * FROM Departments WHERE Id = @Id";
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Department>(query, new { Id = id });
    }

    public async Task<IEnumerable<Department>> GetAllAsync()
    {
        var query = "SELECT * FROM Departments ORDER BY Name";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Department>(query);
    }

    public async Task<IEnumerable<Department>> GetActiveAsync()
    {
        var query = "SELECT * FROM Departments WHERE IsActive = 1 ORDER BY Name";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Department>(query);
    }

    public async Task<int> CreateAsync(Department department)
    {
        var query = @"
            INSERT INTO Departments (Name, Description, IsActive, CreatedAt)
            VALUES (@Name, @Description, @IsActive, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() as int)";

        using var connection = _context.CreateConnection();
        return await connection.QuerySingleAsync<int>(query, department);
    }

    public async Task<bool> UpdateAsync(Department department)
    {
        var query = @"
            UPDATE Departments 
            SET Name = @Name,
                Description = @Description,
                IsActive = @IsActive
            WHERE Id = @Id";

        using var connection = _context.CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(query, department);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var query = "DELETE FROM Departments WHERE Id = @Id";
        using var connection = _context.CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
        return rowsAffected > 0;
    }
}
