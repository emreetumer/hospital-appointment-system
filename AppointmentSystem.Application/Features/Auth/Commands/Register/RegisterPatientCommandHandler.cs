using AppointmentSystem.Application.Contracts.Repositories;
using AppointmentSystem.Application.Contracts.Services;
using AppointmentSystem.Domain.Common;
using AppointmentSystem.Domain.Entities;
using AppointmentSystem.Domain.Enums;
using MediatR;

namespace AppointmentSystem.Application.Features.Auth.Commands.Register;

public class RegisterPatientCommandHandler : IRequestHandler<RegisterPatientCommand, Result<int>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IAuthService _authService;

    public RegisterPatientCommandHandler(
        IUserRepository userRepository,
        IPatientRepository patientRepository,
        IAuthService authService)
    {
        _userRepository = userRepository;
        _patientRepository = patientRepository;
        _authService = authService;
    }

    public async Task<Result<int>> Handle(RegisterPatientCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.ExistsAsync(request.Email);
        if (existingUser)
        {
            return Result<int>.Failure("Email already exists");
        }

        var passwordHash = await _authService.HashPasswordAsync(request.Password);

        var user = new User
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Role = UserRoles.Patient,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var userId = await _userRepository.CreateAsync(user);

        var patient = new Patient
        {
            UserId = userId,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Address = request.Address,
            BloodType = request.BloodType,
            CreatedAt = DateTime.UtcNow
        };

        var patientId = await _patientRepository.CreateAsync(patient);

        return Result<int>.Success(userId, "Patient registered successfully");
    }
}
