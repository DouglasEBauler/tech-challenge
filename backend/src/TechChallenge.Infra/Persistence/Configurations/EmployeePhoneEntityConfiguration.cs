using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechChallenge.Domain.Entities;

namespace TechChallenge.Infra.Persistence.Configurations;

public class EmployeePhoneEntityConfiguration 
    : IEntityTypeConfiguration<EmployeePhoneEntity>
{
    public void Configure(EntityTypeBuilder<EmployeePhoneEntity> builder)
    {
        builder.ToTable("EMPLOYEE_PHONE");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Number)
            .HasColumnName("NUMBER")
            .IsRequired();

        builder.Property(x => x.Type)
            .HasColumnName("TYPE")
            .IsRequired();
        
        builder.Property(p => p.EmployeeId)
            .HasColumnName("EMPLOYEE_ID")
            .IsRequired();

        builder.HasOne(p => p.Employee)
            .WithMany(e => e.Phones)
            .HasForeignKey(p => p.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
