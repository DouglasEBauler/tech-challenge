using System.Text.Json.Serialization;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Commands.UpdateEmployee;

public record UpdateEmployeeCommand(int EmployeeId, string FirstName, string LastName, string Email, string DocumentNumber, DateTime BirthDate, EmployeeRoleType Role, List<UpdateEmployeePhoneCommand> Phones)
    : IAuthCommand<UpdateEmployeeCommandResult>
{
    [JsonIgnore]
    public EmployeeRoleType AuthRole { get; set; }
    
    [JsonIgnore]
    public int AuthEmployeeId { get; set; }
}
