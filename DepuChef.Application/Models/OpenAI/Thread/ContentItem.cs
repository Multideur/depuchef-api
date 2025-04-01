using DepuChef.Application.Models.OpenAI.Message;
using System.Text.Json.Serialization;

namespace DepuChef.Application.Models.OpenAI.Thread;

public class ContentItem
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    [JsonPropertyName("text")]
    public Text? Text { get; set; }
    [JsonPropertyName("image_file")]
    public ImageFileReference? ImageFile { get; set; }
    [JsonPropertyName("image_url")]
    public ImageFileReference? ImageUrl { get; set; }
}
