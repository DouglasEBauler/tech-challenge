using TechChallenge.Domain.Entities;

namespace TechChallenge.Domain.Interfaces;

public interface IEmployeeQueryStore
{
    Task<EmployeeEntity?> GetByIdAsync(int id);
    Task<EmployeeEntity?> GetByEmailAsync(string email);
    Task<List<EmployeeEntity>?> GetAllAsync(int managerId);
    Task<bool> GetEmployeeExistsById(int employeeId);
    Task<bool> GetEmailExists(string email);
    Task<bool> GetDocumentNumberExists(string documentNumberIndex);
}
