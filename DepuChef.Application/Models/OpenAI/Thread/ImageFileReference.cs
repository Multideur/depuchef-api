using System.Text.Json.Serialization;

namespace DepuChef.Application.Models.OpenAI.Thread;

public class ImageFileReference
{
    [JsonPropertyName("file_id")]
    public string? FileId { get; set; }
}