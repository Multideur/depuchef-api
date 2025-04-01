namespace DepuChef.Application.Models;

public class RecipeProcess : AuditedModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string ThreadId { get; set; }
    public string? FileId { get; set; }
}
