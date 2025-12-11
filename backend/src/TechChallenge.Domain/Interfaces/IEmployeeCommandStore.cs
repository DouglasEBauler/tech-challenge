using TechChallenge.Domain.Entities;

namespace TechChallenge.Domain.Interfaces;

public interface IEmployeeCommandStore
{
    Task AddAsync(EmployeeEntity employeeEntity);
    Task UpdateAsync(EmployeeEntity employeeEntity);
    Task DeleteAsync(EmployeeEntity employeeEntity);
}
