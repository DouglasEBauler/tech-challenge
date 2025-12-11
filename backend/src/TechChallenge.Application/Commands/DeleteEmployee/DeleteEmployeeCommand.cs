using System.Text.Json.Serialization;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Commands.DeleteEmployee;

public record DeleteEmployeeCommand(int EmployeeId)
    : IAuthCommand<DeleteEmployeeCommanResult>
{
    [JsonIgnore]
    public EmployeeRoleType AuthRole { get; set; }
    
    [JsonIgnore]
    public int AuthEmployeeId { get; set; }
}