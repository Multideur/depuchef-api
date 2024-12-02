using System.Text.Json.Serialization;
using DepuChef.Application.Models.OpenAI.Thread;

namespace DepuChef.Application.Models.OpenAI.Message;

public class Message
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("role")]
    public string? Role { get; set; }
    [JsonPropertyName("content")]
    public ContentItem[]? Content { get; set; }
    [JsonPropertyName("object")]
    public string? Object { get; set; }
    [JsonPropertyName("created_at")]
    public long CreatedAt { get; set; }
    [JsonPropertyName("thread_id")]
    public string? ThreadId { get; set; }
    [JsonPropertyName("assistant_id")]
    public string? AssistantId { get; set; }
    [JsonPropertyName("run_id")]
    public string? RunId { get; set; }
    [JsonPropertyName("attachments")]
    public object[]? Attachments { get; set; }
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }
}