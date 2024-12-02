namespace DepuChef.Application.Models.OpenAI.CleanUp;

public class CleanUpRequest
{
    public required string FileId { get; set; }
    public required string ThreadId { get; set; }
}
