using System.Text.Json.Serialization;
using System.Text.Json;

namespace DepuChef.Application.Utilities;

public class UnixTimestampToDateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException("Expected a number representing the Unix timestamp.");
        }

        long unixTime = reader.GetInt64();
        return DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime;
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            long unixTime = ((DateTimeOffset)value.Value).ToUnixTimeSeconds();
            writer.WriteNumberValue(unixTime);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}