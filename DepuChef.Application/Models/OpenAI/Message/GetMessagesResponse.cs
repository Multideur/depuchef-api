using System.Text.Json.Serialization;

namespace DepuChef.Application.Models.OpenAI.Message;

public class GetMessagesResponse
{
    [JsonPropertyName("object")]
    public string? Object { get; set; }
    [JsonPropertyName("data")]
    public List<Message>? Data { get; set; }
}
