using AppointmentSystem.Domain.Common;
using MediatR;

namespace AppointmentSystem.Application.Features.Auth.Commands.Register;

public class RegisterPatientCommand : IRequest<Result<int>>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; }
    public string Address { get; set; }
    public string BloodType { get; set; }
}
