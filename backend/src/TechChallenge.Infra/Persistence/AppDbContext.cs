using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Entities;

namespace TechChallenge.Infra.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> opts) : DbContext(opts)
{
    public DbSet<EmployeeEntity> Employees { get; set; }
    public DbSet<EmployeePhoneEntity> EmployeePhones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
