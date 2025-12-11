using System.Text.Json.Serialization;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Queries.GetAllEmployes;

public record GetAllCommand : IAuthCommand<GetAllCommandResult>
{
    [JsonIgnore]
    public EmployeeRoleType AuthRole { get; set; }

    [JsonIgnore]
    public int AuthEmployeeId { get ; set; }
}
