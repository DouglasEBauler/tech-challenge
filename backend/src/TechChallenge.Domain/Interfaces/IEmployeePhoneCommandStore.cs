using TechChallenge.Domain.Entities;

namespace TechChallenge.Domain.Interfaces;

public interface IEmployeePhoneCommandStore
{
    Task AddAsync(EmployeePhoneEntity employeeEntity);
    Task UpdateAsync(EmployeePhoneEntity employeeEntity);
}
