using AppointmentSystem.Domain.Common;

namespace AppointmentSystem.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Role { get; set; } // Patient, Doctor, Admin
    public bool IsActive { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
