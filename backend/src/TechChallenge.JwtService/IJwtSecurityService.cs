using TechChallenge.Domain.Entities;

namespace TechChallenge.JwtService;

public interface IJwtSecurityService
{
    string GenerateToken(EmployeeEntity user);

    bool ValidateToken(string token);
}