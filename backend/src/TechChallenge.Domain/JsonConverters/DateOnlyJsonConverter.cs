using System.Text.Json;
using System.Text.Json.Serialization;

namespace TechChallenge.Domain.JsonConverters;

public class DateOnlyJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (DateTime.TryParse(value, out var date))
            return date.Date;

        throw new JsonException("Invalid date format. Use 'YYYY-MM-DD'.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
}
