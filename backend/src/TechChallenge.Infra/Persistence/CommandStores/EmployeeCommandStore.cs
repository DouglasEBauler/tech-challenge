using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Infra.Persistence.CommandStores;

public class EmployeeCommandStore(AppDbContext db) 
    : IEmployeeCommandStore
{
    private readonly AppDbContext _db = db;

    public async Task AddAsync(EmployeeEntity employeeEntity)
    {
        await _db.Employees.AddAsync(employeeEntity);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(EmployeeEntity employeeEntity)
    {
        _db.EmployeePhones.RemoveRange(
            _db.EmployeePhones.Where(p => p.EmployeeId == employeeEntity.Id)
        );

        _db.Employees.Update(employeeEntity);

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(EmployeeEntity employeeEntity)
    {
        _db.Employees.Remove(employeeEntity);
        await _db.SaveChangesAsync();
    }
}
