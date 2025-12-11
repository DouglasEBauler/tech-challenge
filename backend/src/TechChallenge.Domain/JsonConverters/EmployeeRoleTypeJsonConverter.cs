using System.Text.Json;
using System.Text.Json.Serialization;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Domain.JsonConverters;

public class EmployeeRoleTypeJsonConverter : JsonConverter<EmployeeRoleType>
{
    public override EmployeeRoleType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (Enum.TryParse<EmployeeRoleType>(value, true, out var result))
            return result;

        throw new JsonException($"Employee role type invalid '{value}'. Allowed: User, Leader, Director.");
    }

    public override void Write(Utf8JsonWriter writer, EmployeeRoleType value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
}
