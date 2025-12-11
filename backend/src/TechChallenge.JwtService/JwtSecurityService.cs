using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TechChallenge.Domain.Entities;

namespace TechChallenge.JwtService;

public class JwtSecurityService(JwtSettings settings) : IJwtSecurityService
{
    private readonly JwtSettings _settings = settings;

    public string GenerateToken(EmployeeEntity user)
        => new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims:
            [
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(ClaimTypes.Role, user.Role.ToString()),
            ],
            expires: DateTime.UtcNow.AddHours(_settings.ExpireHours),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key)), SecurityAlgorithms.HmacSha256)
        ));

    public bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _settings.Issuer,
                ValidAudience = _settings.Audience,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
