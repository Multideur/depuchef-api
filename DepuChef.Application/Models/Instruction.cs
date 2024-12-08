namespace DepuChef.Application.Models;

public class Instruction
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public int Step { get; set; }
    public string? Description { get; set; }
}
