namespace DepuChef.Application.Models.OpenAI.CleanUp;

public class CleanUpRequest
{
    public string? FileId { get; set; }
    public required string ThreadId { get; set; }
}
