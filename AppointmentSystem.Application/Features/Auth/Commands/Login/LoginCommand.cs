using AppointmentSystem.Domain.Common;
using MediatR;

namespace AppointmentSystem.Application.Features.Auth.Commands.Login;

public class LoginCommand : IRequest<Result<LoginResponse>>
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginResponse
{
    public int UserId { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; }
    public string Token { get; set; }
}
