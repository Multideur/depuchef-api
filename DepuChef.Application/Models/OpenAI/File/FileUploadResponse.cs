using System.Text.Json.Serialization;

namespace DepuChef.Application.Models.OpenAI.File;

public class FileUploadResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("purpose")]
    public string? Purpose { get; set; }
}
