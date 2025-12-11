using System.Text.Json.Serialization;
using TechChallenge.Domain.JsonConverters;

namespace TechChallenge.Domain.Enums;

[JsonConverter(typeof(EmployeeRoleTypeJsonConverter))]
public enum EmployeeRoleType
{
    User = 0,
    Leader = 1,
    Director = 2,
    Admin = 3
}
