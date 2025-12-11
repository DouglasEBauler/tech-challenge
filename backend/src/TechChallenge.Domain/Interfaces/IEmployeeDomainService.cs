using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Domain.Interfaces;

public interface IEmployeeDomainService
{
    EmployeeEntity CreateEmployee(string firstName, string lastName, string email, string documentNumber, string password, DateTime birthDate, EmployeeRoleType employeeRoleType, Dictionary<string, PhoneType> phones, EmployeeRoleType userAuthRole, int authEmployeeId);
    Task<EmployeeEntity> UpdateEmployee(int employeeId, string firstName, string lastName, string email, DateTime birthDate, Dictionary<string, PhoneType> phones, string documentNumber, EmployeeRoleType userAuthRole);
    Task<EmployeeEntity> DeleteEmployeeAsync(int employeeId, EmployeeRoleType userAuthRole);
    Task<List<EmployeeEntity>> GetAllAsync(EmployeeRoleType userAuthRole, int authEmployeeId);
    Task<EmployeeEntity> GetByIdAsync(int employeeId, EmployeeRoleType userAuthRole);
    Task<EmployeeEntity> LoginEmployeeAsync(string email, string password);
}
