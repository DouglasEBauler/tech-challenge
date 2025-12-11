using TechChallenge.Domain.Entities;

namespace TechChallenge.Domain.Interfaces;

public interface IEmployeePhoneQueryStore
{
    Task<List<EmployeePhoneEntity>?> GetPhonesByEmployeeId(int employeeId);
}
