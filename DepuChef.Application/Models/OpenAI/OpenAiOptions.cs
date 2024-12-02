namespace DepuChef.Application.Models.OpenAI;

public class OpenAiOptions
{
    public const string Options = "OpenAi";
    public string? BaseUrl { get; set; }
    public string? ApiKey { get; set; }
    public string? AssistantId { get; set; }
}
