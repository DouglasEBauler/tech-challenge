using System.Text.Json;
using System.Text.Json.Serialization;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Domain.JsonConverters;

public class PhoneTypeJsonConverter : JsonConverter<PhoneType>
{
    public override PhoneType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (Enum.TryParse<PhoneType>(value, true, out var result))
            return result;

        throw new JsonException($"Phone type invalid '{value}'. Allowed: Mobile, Landline, Work, Home, Fax, Emergency or Other.");
    }

    public override void Write(Utf8JsonWriter writer, PhoneType value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());

}
