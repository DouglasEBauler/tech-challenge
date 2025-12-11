using System.Security.Claims;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Extensions;

public static class ClaimsPrincipalExtension
{
    public static EmployeeRoleType GetEmployeeRole(this ClaimsPrincipal user)
    {
        var claimValue = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        if (Enum.TryParse<EmployeeRoleType>(claimValue, true, out var role))
            return role;

        return EmployeeRoleType.User;
    }

    public static int GetEmployeeId(this ClaimsPrincipal user)
    {
        var claimValue = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (int.TryParse(claimValue, out var employeeId))
            return employeeId;

        return 0;
    }
}
