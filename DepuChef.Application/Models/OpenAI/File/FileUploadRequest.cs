using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace DepuChef.Application.Models.OpenAI.File;

public class FileUploadRequest
{
    [JsonPropertyName("purpose")]
    public string? Purpose { get; set; }
    [JsonPropertyName("file")]
    public IFormFile? File { get; set; }
    public MemoryStream? Stream { get; set; }
}