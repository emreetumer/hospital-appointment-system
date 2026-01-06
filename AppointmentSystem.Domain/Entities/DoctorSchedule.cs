using AppointmentSystem.Domain.Common;

namespace AppointmentSystem.Domain.Entities;

public class DoctorSchedule : BaseEntity
{
    public int DoctorId { get; set; }
    public byte DayOfWeek { get; set; } // 1=Monday, 7=Sunday
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsActive { get; set; }

    // Navigation property
    public Doctor Doctor { get; set; }
}
