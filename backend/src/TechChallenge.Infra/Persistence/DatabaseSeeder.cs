using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Helpers;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Infra.Persistence;

public class DatabaseSeeder(AppDbContext db) 
    : IDatabaseSeeder
{
    private readonly AppDbContext _db = db;

    public async Task SeedAsync()
    {
        await _db.Database.MigrateAsync();

        if (await _db.Employees.AnyAsync(e => e.Role == EmployeeRoleType.Admin))
            return;

        const string defaultDoc = "00000000000";

        var (hash, salt) = PasswordHelper.CreateHashPassword("Admin@123");
        var docEncrypt = EncryptionHelper.EncryptDocumentNumber(defaultDoc);

        _db.Employees.Add(new EmployeeEntity
        {
            FirstName = "System",
            LastName = "Admin",
            Email = "admin@company.com",
            DocumentNumber = docEncrypt,
            DocumentNumberIndex = EncryptionHelper.CreateIndexHash(defaultDoc),
            BirthDate = DateTime.UtcNow.AddYears(-30),
            Role = EmployeeRoleType.Admin,
            PasswordHash = hash,
            PasswordSalt = salt,
            Phones =
            [
                new EmployeePhoneEntity { Number = "+5500000000000", Type = PhoneType.Mobile }
            ]
        });

        await _db.SaveChangesAsync();
    }
}
