using System.Text.Json.Serialization;

namespace DepuChef.Application.Models.OpenAI.Thread;

public class ThreadRequest
{
    [JsonPropertyName("assistant_id")]
    public string? AssistantId { get; set; }
    [JsonPropertyName("thread")]
    public AiThread? Thread { get; set; }
}
