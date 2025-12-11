using TechChallenge.Domain.Enums;

namespace TechChallenge.Domain.Entities;

public class EmployeePhoneEntity
{
    public int Id { get; set; }
    public string Number { get; set; } = null!;
    public PhoneType Type { get; set; }
    public int EmployeeId { get; set; }
    public EmployeeEntity Employee { get; set; } = null!;
}
