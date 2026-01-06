using AppointmentSystem.Domain.Common;

namespace AppointmentSystem.Domain.Entities;

public class Appointment : BaseEntity
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan AppointmentTime { get; set; }
    public string Status { get; set; } // Pending, Confirmed, Cancelled, Completed, NoShow
    public string Notes { get; set; }
    public string CancellationReason { get; set; }

    // Navigation properties
    public Patient Patient { get; set; }
    public Doctor Doctor { get; set; }
}
