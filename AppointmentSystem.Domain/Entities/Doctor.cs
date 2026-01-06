using AppointmentSystem.Domain.Common;

namespace AppointmentSystem.Domain.Entities;

public class Doctor : BaseEntity
{
    public int UserId { get; set; }
    public int DepartmentId { get; set; }
    public string Title { get; set; }
    public string LicenseNumber { get; set; }
    public string Biography { get; set; }
    public int? ExperienceYears { get; set; }
    public bool IsActive { get; set; }

    // Navigation properties (will be populated via Dapper joins)
    public User User { get; set; }
    public Department Department { get; set; }
}
