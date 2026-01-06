using AppointmentSystem.Domain.Common;

namespace AppointmentSystem.Domain.Entities;

public class MedicalRecord : BaseEntity
{
    public int AppointmentId { get; set; }
    public string Diagnosis { get; set; }
    public string Treatment { get; set; }
    public string Prescription { get; set; }
    public string Notes { get; set; }

    // Navigation property
    public Appointment Appointment { get; set; }
}
