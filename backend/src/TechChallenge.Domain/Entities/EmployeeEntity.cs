using TechChallenge.Domain.Enums;

namespace TechChallenge.Domain.Entities;

public class EmployeeEntity
{
    public int Id { get; set; }

    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string DocumentNumber { get; set; } = default!;
    public string DocumentNumberIndex { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string PasswordSalt { get; set; } = default!;
    public DateTime BirthDate { get; set; }
    public List<EmployeePhoneEntity> Phones { get; set; } = [];
    public int? ManagerId { get; set; }
    public EmployeeEntity? Manager { get; set; }
    public EmployeeRoleType Role { get; set; }
}
