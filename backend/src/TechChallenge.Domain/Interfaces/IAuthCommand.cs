using System.Text.Json.Serialization;
using TechChallenge.Domain.Enums;
namespace TechChallenge.Domain.Interfaces;

public interface IAuthCommand<TResponse> : ICommand<TResponse>
    where TResponse : ICommandResult
{
    [JsonIgnore]
    EmployeeRoleType AuthRole { get; set; }

    [JsonIgnore]
    int AuthEmployeeId { get; set; }
}
