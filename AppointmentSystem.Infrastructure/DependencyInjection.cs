using AppointmentSystem.Application.Contracts.Repositories;
using AppointmentSystem.Application.Contracts.Services;
using AppointmentSystem.Infrastructure.Data;
using AppointmentSystem.Infrastructure.Repositories;
using AppointmentSystem.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<DapperContext>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
