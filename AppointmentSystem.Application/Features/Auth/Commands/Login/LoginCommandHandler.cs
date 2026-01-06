using AppointmentSystem.Application.Contracts.Repositories;
using AppointmentSystem.Application.Contracts.Services;
using AppointmentSystem.Domain.Common;
using MediatR;

namespace AppointmentSystem.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public LoginCommandHandler(IUserRepository userRepository, IAuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
        {
            return Result<LoginResponse>.Failure("Invalid email or password");
        }

        if (!user.IsActive)
        {
            return Result<LoginResponse>.Failure("User account is inactive");
        }

        var isValidPassword = await _authService.ValidatePasswordAsync(request.Password, user.PasswordHash);

        if (!isValidPassword)
        {
            return Result<LoginResponse>.Failure("Invalid email or password");
        }

        var token = await _authService.GenerateJwtTokenAsync(user.Id, user.Email, user.Role);

        var response = new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            Token = token
        };

        return Result<LoginResponse>.Success(response, "Login successful");
    }
}
