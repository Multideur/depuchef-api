using System.Text.Json.Serialization;

namespace DepuChef.Application.Models.OpenAI.Thread;

public class DeleteThreadResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("deleted")]
    public bool Deleted { get; set; }
}
