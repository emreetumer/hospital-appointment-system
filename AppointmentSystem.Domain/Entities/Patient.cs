using AppointmentSystem.Domain.Common;

namespace AppointmentSystem.Domain.Entities;

public class Patient : BaseEntity
{
    public int UserId { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; }
    public string Address { get; set; }
    public string EmergencyContact { get; set; }
    public string BloodType { get; set; }
    public string Allergies { get; set; }

    // Navigation property
    public User User { get; set; }
}
