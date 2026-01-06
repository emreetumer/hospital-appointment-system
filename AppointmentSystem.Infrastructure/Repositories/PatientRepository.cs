using AppointmentSystem.Application.Contracts.Repositories;
using AppointmentSystem.Domain.Entities;
using AppointmentSystem.Infrastructure.Data;
using Dapper;

namespace AppointmentSystem.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly DapperContext _context;

    public PatientRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Patient> GetByIdAsync(int id)
    {
        var query = @"
            SELECT p.*, u.*
            FROM Patients p
            INNER JOIN Users u ON p.UserId = u.Id
            WHERE p.Id = @Id";

        using var connection = _context.CreateConnection();
        var patient = await connection.QueryAsync<Patient, User, Patient>(
            query,
            (patient, user) =>
            {
                patient.User = user;
                return patient;
            },
            new { Id = id },
            splitOn: "Id"
        );

        return patient.FirstOrDefault();
    }

    public async Task<Patient> GetByUserIdAsync(int userId)
    {
        var query = @"
            SELECT p.*, u.*
            FROM Patients p
            INNER JOIN Users u ON p.UserId = u.Id
            WHERE p.UserId = @UserId";

        using var connection = _context.CreateConnection();
        var patient = await connection.QueryAsync<Patient, User, Patient>(
            query,
            (patient, user) =>
            {
                patient.User = user;
                return patient;
            },
            new { UserId = userId },
            splitOn: "Id"
        );

        return patient.FirstOrDefault();
    }

    public async Task<IEnumerable<Patient>> GetAllAsync()
    {
        var query = @"
            SELECT p.*, u.*
            FROM Patients p
            INNER JOIN Users u ON p.UserId = u.Id
            ORDER BY u.FirstName, u.LastName";

        using var connection = _context.CreateConnection();
        var patients = await connection.QueryAsync<Patient, User, Patient>(
            query,
            (patient, user) =>
            {
                patient.User = user;
                return patient;
            },
            splitOn: "Id"
        );

        return patients;
    }

    public async Task<int> CreateAsync(Patient patient)
    {
        var query = @"
            INSERT INTO Patients (UserId, DateOfBirth, Gender, Address, EmergencyContact, BloodType, Allergies, CreatedAt)
            VALUES (@UserId, @DateOfBirth, @Gender, @Address, @EmergencyContact, @BloodType, @Allergies, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() as int)";

        using var connection = _context.CreateConnection();
        return await connection.QuerySingleAsync<int>(query, patient);
    }

    public async Task<bool> UpdateAsync(Patient patient)
    {
        var query = @"
            UPDATE Patients 
            SET DateOfBirth = @DateOfBirth,
                Gender = @Gender,
                Address = @Address,
                EmergencyContact = @EmergencyContact,
                BloodType = @BloodType,
                Allergies = @Allergies,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        using var connection = _context.CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(query, patient);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var query = "DELETE FROM Patients WHERE Id = @Id";
        using var connection = _context.CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
        return rowsAffected > 0;
    }
}
