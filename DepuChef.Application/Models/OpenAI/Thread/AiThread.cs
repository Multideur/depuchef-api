using System.Text.Json.Serialization;

namespace DepuChef.Application.Models.OpenAI.Thread;

public class AiThread
{
    [JsonPropertyName("messages")]
    public List<Message.Message>? Messages { get; set; }
}
