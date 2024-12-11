namespace DepuChef.Api.Models;

public class InstructionDto
{
    public Guid RecipeId { get; set; }
    public int Step { get; set; }
    public string? Description { get; set; }
}
