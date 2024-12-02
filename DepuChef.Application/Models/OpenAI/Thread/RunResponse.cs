using DepuChef.Application.Utilities;
using System.Text.Json.Serialization;

namespace DepuChef.Application.Models.OpenAI.Thread;

public class RunResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("thread_id")]
    public string? ThreadId { get; set; }
    [JsonPropertyName("status")]
    public string? Status { get; set; }
    [JsonPropertyName("created_at")]
    [JsonConverter(typeof(UnixTimestampToDateTimeConverter))]
    public DateTime? CreatedAt { get; set; }
    [JsonPropertyName("started_at")]
    [JsonConverter(typeof(UnixTimestampToDateTimeConverter))]
    public DateTime? StartedAt { get; set; }
    [JsonPropertyName("completed_at")]
    [JsonConverter(typeof(UnixTimestampToDateTimeConverter))]
    public DateTime? CompletedAt { get; set; }
}
