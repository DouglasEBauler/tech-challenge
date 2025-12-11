using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Queries.GetAllEmployes;

public record GetAllEmployeeCommandResult(int EmployeeId, string FirstName, string LastName, string Email, DateTime BirthDate, List<string> Phones, string ManagerName, EmployeeRoleType Role);
