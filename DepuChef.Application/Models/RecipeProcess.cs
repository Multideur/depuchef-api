namespace DepuChef.Application.Models;

public class RecipeProcess : AuditedModel
{
    public Guid Id { get; set; }
    public required string ThreadId { get; set; }
    public required string FileId { get; set; }
}
