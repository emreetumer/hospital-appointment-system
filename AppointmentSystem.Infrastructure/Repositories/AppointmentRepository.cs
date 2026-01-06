using AppointmentSystem.Application.Contracts.Repositories;
using AppointmentSystem.Domain.Entities;
using AppointmentSystem.Infrastructure.Data;
using Dapper;

namespace AppointmentSystem.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly DapperContext _context;

    public AppointmentRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Appointment> GetByIdAsync(int id)
    {
        var query = @"
            SELECT a.*, p.*, u1.*, d.*, u2.*, dep.*
            FROM Appointments a
            INNER JOIN Patients p ON a.PatientId = p.Id
            INNER JOIN Users u1 ON p.UserId = u1.Id
            INNER JOIN Doctors d ON a.DoctorId = d.Id
            INNER JOIN Users u2 ON d.UserId = u2.Id
            INNER JOIN Departments dep ON d.DepartmentId = dep.Id
            WHERE a.Id = @Id";

        using var connection = _context.CreateConnection();
        var appointments = await connection.QueryAsync<Appointment, Patient, User, Doctor, User, Department, Appointment>(
            query,
            (appointment, patient, patientUser, doctor, doctorUser, department) =>
            {
                patient.User = patientUser;
                doctor.User = doctorUser;
                doctor.Department = department;
                appointment.Patient = patient;
                appointment.Doctor = doctor;
                return appointment;
            },
            new { Id = id },
            splitOn: "Id,Id,Id,Id,Id"
        );

        return appointments.FirstOrDefault();
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync()
    {
        var query = @"
            SELECT a.*, p.*, u1.*, d.*, u2.*, dep.*
            FROM Appointments a
            INNER JOIN Patients p ON a.PatientId = p.Id
            INNER JOIN Users u1 ON p.UserId = u1.Id
            INNER JOIN Doctors d ON a.DoctorId = d.Id
            INNER JOIN Users u2 ON d.UserId = u2.Id
            INNER JOIN Departments dep ON d.DepartmentId = dep.Id
            ORDER BY a.AppointmentDate DESC, a.AppointmentTime DESC";

        using var connection = _context.CreateConnection();
        var appointments = await connection.QueryAsync<Appointment, Patient, User, Doctor, User, Department, Appointment>(
            query,
            (appointment, patient, patientUser, doctor, doctorUser, department) =>
            {
                patient.User = patientUser;
                doctor.User = doctorUser;
                doctor.Department = department;
                appointment.Patient = patient;
                appointment.Doctor = doctor;
                return appointment;
            },
            splitOn: "Id,Id,Id,Id,Id"
        );

        return appointments;
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId)
    {
        var query = @"
            SELECT a.*, p.*, u1.*, d.*, u2.*, dep.*
            FROM Appointments a
            INNER JOIN Patients p ON a.PatientId = p.Id
            INNER JOIN Users u1 ON p.UserId = u1.Id
            INNER JOIN Doctors d ON a.DoctorId = d.Id
            INNER JOIN Users u2 ON d.UserId = u2.Id
            INNER JOIN Departments dep ON d.DepartmentId = dep.Id
            WHERE a.PatientId = @PatientId
            ORDER BY a.AppointmentDate DESC, a.AppointmentTime DESC";

        using var connection = _context.CreateConnection();
        var appointments = await connection.QueryAsync<Appointment, Patient, User, Doctor, User, Department, Appointment>(
            query,
            (appointment, patient, patientUser, doctor, doctorUser, department) =>
            {
                patient.User = patientUser;
                doctor.User = doctorUser;
                doctor.Department = department;
                appointment.Patient = patient;
                appointment.Doctor = doctor;
                return appointment;
            },
            new { PatientId = patientId },
            splitOn: "Id,Id,Id,Id,Id"
        );

        return appointments;
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId)
    {
        var query = @"
            SELECT a.*, p.*, u1.*, d.*, u2.*, dep.*
            FROM Appointments a
            INNER JOIN Patients p ON a.PatientId = p.Id
            INNER JOIN Users u1 ON p.UserId = u1.Id
            INNER JOIN Doctors d ON a.DoctorId = d.Id
            INNER JOIN Users u2 ON d.UserId = u2.Id
            INNER JOIN Departments dep ON d.DepartmentId = dep.Id
            WHERE a.DoctorId = @DoctorId
            ORDER BY a.AppointmentDate DESC, a.AppointmentTime DESC";

        using var connection = _context.CreateConnection();
        var appointments = await connection.QueryAsync<Appointment, Patient, User, Doctor, User, Department, Appointment>(
            query,
            (appointment, patient, patientUser, doctor, doctorUser, department) =>
            {
                patient.User = patientUser;
                doctor.User = doctorUser;
                doctor.Department = department;
                appointment.Patient = patient;
                appointment.Doctor = doctor;
                return appointment;
            },
            new { DoctorId = doctorId },
            splitOn: "Id,Id,Id,Id,Id"
        );

        return appointments;
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorAndDateAsync(int doctorId, DateTime date)
    {
        var query = @"
            SELECT * FROM Appointments
            WHERE DoctorId = @DoctorId AND AppointmentDate = @Date
            ORDER BY AppointmentTime";

        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Appointment>(query, new { DoctorId = doctorId, Date = date });
    }

    public async Task<int> CreateAsync(Appointment appointment)
    {
        var query = @"
            INSERT INTO Appointments (PatientId, DoctorId, AppointmentDate, AppointmentTime, Status, Notes, CreatedAt)
            VALUES (@PatientId, @DoctorId, @AppointmentDate, @AppointmentTime, @Status, @Notes, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() as int)";

        using var connection = _context.CreateConnection();
        return await connection.QuerySingleAsync<int>(query, appointment);
    }

    public async Task<bool> UpdateAsync(Appointment appointment)
    {
        var query = @"
            UPDATE Appointments 
            SET Status = @Status,
                Notes = @Notes,
                CancellationReason = @CancellationReason,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        using var connection = _context.CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(query, appointment);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var query = "DELETE FROM Appointments WHERE Id = @Id";
        using var connection = _context.CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<bool> IsTimeSlotAvailableAsync(int doctorId, DateTime date, TimeSpan time, int? excludeAppointmentId = null)
    {
        var query = @"
            SELECT COUNT(1) 
            FROM Appointments 
            WHERE DoctorId = @DoctorId 
                AND AppointmentDate = @Date 
                AND AppointmentTime = @Time
                AND Status NOT IN ('Cancelled', 'NoShow')
                AND (@ExcludeId IS NULL OR Id != @ExcludeId)";

        using var connection = _context.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(query, new 
        { 
            DoctorId = doctorId, 
            Date = date, 
            Time = time,
            ExcludeId = excludeAppointmentId 
        });

        return count == 0;
    }
}
