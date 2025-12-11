using System.Text.Json.Serialization;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Commands.CreateEmployee;

public record CreateEmployeeCommand(string FirstName, string LastName, string Email, string DocumentNumber, DateTime BirthDate, List<CreateEmployeePhoneCommand> Phones, EmployeeRoleType Role, string Password)
    : IAuthCommand<CreateEmployeeCommandResult>
{
    [JsonIgnore]
    public EmployeeRoleType AuthRole { get; set; }

    [JsonIgnore]
    public int AuthEmployeeId { get; set; }
}
