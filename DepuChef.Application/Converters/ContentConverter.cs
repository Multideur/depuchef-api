using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using DepuChef.Application.Models.OpenAI.Message;
using DepuChef.Application.Models.OpenAI.Thread;

namespace DepuChef.Application.Converters;

public class ContentConverter : JsonConverter<object>
{
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            return JsonSerializer.Deserialize<ContentItem[]>(ref reader, options);
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString();
        }
        throw new JsonException("Unexpected token type for content property.");
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        if (value is ContentItem[] contentItems)
        {
            JsonSerializer.Serialize(writer, contentItems, options);
        }
        else if (value is string stringContent)
        {
            writer.WriteStringValue(stringContent);
        }
        else
        {
            throw new JsonException("Unexpected value type for content property.");
        }
    }
}
