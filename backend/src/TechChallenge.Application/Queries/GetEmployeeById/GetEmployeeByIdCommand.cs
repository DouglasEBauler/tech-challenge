using System.Text.Json.Serialization;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Queries.GetEmployeeById;

public record GetEmployeeByIdCommand(int Id) : IAuthCommand<GetEmployeeByIdCommandResult>
{
    [JsonIgnore]
    public EmployeeRoleType AuthRole { get; set; }

    [JsonIgnore]
    public int AuthEmployeeId { get; set; }
}
