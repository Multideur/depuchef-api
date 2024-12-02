using System.Text.Json.Serialization;

namespace DepuChef.Application.Models.OpenAI.Thread;

public class ThreadResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
}
