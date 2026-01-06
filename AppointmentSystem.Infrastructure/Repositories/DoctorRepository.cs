using AppointmentSystem.Application.Contracts.Repositories;
using AppointmentSystem.Domain.Entities;
using AppointmentSystem.Infrastructure.Data;
using Dapper;

namespace AppointmentSystem.Infrastructure.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly DapperContext _context;

    public DoctorRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Doctor> GetByIdAsync(int id)
    {
        var query = @"
            SELECT d.*, u.*, dep.*
            FROM Doctors d
            INNER JOIN Users u ON d.UserId = u.Id
            INNER JOIN Departments dep ON d.DepartmentId = dep.Id
            WHERE d.Id = @Id";

        using var connection = _context.CreateConnection();
        var doctor = await connection.QueryAsync<Doctor, User, Department, Doctor>(
            query,
            (doctor, user, department) =>
            {
                doctor.User = user;
                doctor.Department = department;
                return doctor;
            },
            new { Id = id },
            splitOn: "Id,Id"
        );

        return doctor.FirstOrDefault();
    }

    public async Task<Doctor> GetByUserIdAsync(int userId)
    {
        var query = @"
            SELECT d.*, u.*, dep.*
            FROM Doctors d
            INNER JOIN Users u ON d.UserId = u.Id
            INNER JOIN Departments dep ON d.DepartmentId = dep.Id
            WHERE d.UserId = @UserId";

        using var connection = _context.CreateConnection();
        var doctor = await connection.QueryAsync<Doctor, User, Department, Doctor>(
            query,
            (doctor, user, department) =>
            {
                doctor.User = user;
                doctor.Department = department;
                return doctor;
            },
            new { UserId = userId },
            splitOn: "Id,Id"
        );

        return doctor.FirstOrDefault();
    }

    public async Task<IEnumerable<Doctor>> GetAllAsync()
    {
        var query = @"
            SELECT d.*, u.*, dep.*
            FROM Doctors d
            INNER JOIN Users u ON d.UserId = u.Id
            INNER JOIN Departments dep ON d.DepartmentId = dep.Id
            WHERE d.IsActive = 1
            ORDER BY u.FirstName, u.LastName";

        using var connection = _context.CreateConnection();
        var doctors = await connection.QueryAsync<Doctor, User, Department, Doctor>(
            query,
            (doctor, user, department) =>
            {
                doctor.User = user;
                doctor.Department = department;
                return doctor;
            },
            splitOn: "Id,Id"
        );

        return doctors;
    }

    public async Task<IEnumerable<Doctor>> GetByDepartmentIdAsync(int departmentId)
    {
        var query = @"
            SELECT d.*, u.*, dep.*
            FROM Doctors d
            INNER JOIN Users u ON d.UserId = u.Id
            INNER JOIN Departments dep ON d.DepartmentId = dep.Id
            WHERE d.DepartmentId = @DepartmentId AND d.IsActive = 1
            ORDER BY u.FirstName, u.LastName";

        using var connection = _context.CreateConnection();
        var doctors = await connection.QueryAsync<Doctor, User, Department, Doctor>(
            query,
            (doctor, user, department) =>
            {
                doctor.User = user;
                doctor.Department = department;
                return doctor;
            },
            new { DepartmentId = departmentId },
            splitOn: "Id,Id"
        );

        return doctors;
    }

    public async Task<int> CreateAsync(Doctor doctor)
    {
        var query = @"
            INSERT INTO Doctors (UserId, DepartmentId, Title, LicenseNumber, Biography, ExperienceYears, IsActive, CreatedAt)
            VALUES (@UserId, @DepartmentId, @Title, @LicenseNumber, @Biography, @ExperienceYears, @IsActive, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() as int)";

        using var connection = _context.CreateConnection();
        return await connection.QuerySingleAsync<int>(query, doctor);
    }

    public async Task<bool> UpdateAsync(Doctor doctor)
    {
        var query = @"
            UPDATE Doctors 
            SET DepartmentId = @DepartmentId,
                Title = @Title,
                Biography = @Biography,
                ExperienceYears = @ExperienceYears,
                IsActive = @IsActive,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        using var connection = _context.CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(query, doctor);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var query = "DELETE FROM Doctors WHERE Id = @Id";
        using var connection = _context.CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
        return rowsAffected > 0;
    }
}
