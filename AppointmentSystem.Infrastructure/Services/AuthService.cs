using AppointmentSystem.Application.Contracts.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AppointmentSystem.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<string> GenerateJwtTokenAsync(int userId, string email, string role)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expiryInMinutes = int.Parse(jwtSettings["ExpiryInMinutes"]);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Task.FromResult(tokenString);
    }

    public Task<bool> ValidatePasswordAsync(string password, string passwordHash)
    {
        var isValid = BCrypt.Net.BCrypt.Verify(password, passwordHash);
        return Task.FromResult(isValid);
    }

    public Task<string> HashPasswordAsync(string password)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        return Task.FromResult(hashedPassword);
    }
}
