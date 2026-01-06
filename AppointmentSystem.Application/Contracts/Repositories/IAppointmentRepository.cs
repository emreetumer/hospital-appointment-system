using AppointmentSystem.Domain.Entities;

namespace AppointmentSystem.Application.Contracts.Repositories;

public interface IAppointmentRepository
{
    Task<Appointment> GetByIdAsync(int id);
    Task<IEnumerable<Appointment>> GetAllAsync();
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
    Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId);
    Task<IEnumerable<Appointment>> GetByDoctorAndDateAsync(int doctorId, DateTime date);
    Task<int> CreateAsync(Appointment appointment);
    Task<bool> UpdateAsync(Appointment appointment);
    Task<bool> DeleteAsync(int id);
    Task<bool> IsTimeSlotAvailableAsync(int doctorId, DateTime date, TimeSpan time, int? excludeAppointmentId = null);
}
