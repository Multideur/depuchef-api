using System.Text.Json.Serialization;

namespace DepuChef.Application.Models.OpenAI.Message;

public class Text
{
    [JsonPropertyName("value")]
    public string? Value { get; set; }
    [JsonPropertyName("annotations")]
    public List<object>? Annotations { get; set; }
}
