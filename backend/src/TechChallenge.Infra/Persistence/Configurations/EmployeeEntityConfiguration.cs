using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechChallenge.Domain.Entities;

namespace TechChallenge.Infra.Persistence.Configurations;

public class EmployeeEntityConfiguration 
    : IEntityTypeConfiguration<EmployeeEntity>
{
    public void Configure(EntityTypeBuilder<EmployeeEntity> builder)
    {
        builder.ToTable("EMPLOYEE");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.FirstName)
            .HasColumnName("FIRST_NAME")
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasColumnName("LAST_NAME")
            .IsRequired();

        builder.Property(x => x.Email)
            .HasColumnName("EMAIL")
            .IsRequired();

        builder.HasIndex(x => x.Email)
            .IsUnique()
            .HasDatabaseName("UX_EMPLOYEE_EMAIL_INDEX");

        builder.Property(x => x.DocumentNumber)
            .HasColumnName("DOCUMENT_NUMBER")
            .IsRequired();

        builder.Property(x => x.DocumentNumberIndex)
            .HasColumnName("DOCUMENT_NUMBER_INDEX")
            .IsRequired();

        builder.HasIndex(x => x.DocumentNumberIndex)
            .IsUnique()
            .HasDatabaseName("UX_EMPLOYEE_DOCUMENT_NUMBER_INDEX");
        
        builder.Property(x => x.Password)
            .HasColumnName("PASSWORD")
            .IsRequired();

        builder.Property(x => x.BirthDate)
            .HasColumnName("BIRTH_DATE")
            .HasColumnType("DATE")
            .IsRequired();

        builder.Property(x => x.Role)
            .HasColumnName("ROLE")
            .IsRequired();

        builder.HasMany(x => x.Phones)
            .WithOne(p => p.Employee)
            .HasForeignKey(p => p.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.ManagerId)
            .HasColumnName("MANAGER_ID");
        
        builder.HasOne(x => x.Manager)
            .WithMany()
            .HasForeignKey(x => x.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
    }

}
