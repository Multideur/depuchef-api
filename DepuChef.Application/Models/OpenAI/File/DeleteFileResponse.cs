using System.Text.Json.Serialization;

namespace DepuChef.Application.Models.OpenAI.File;

public class DeleteFileResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("deleted")]
    public bool Deleted { get; set; }
}
