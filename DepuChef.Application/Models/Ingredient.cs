namespace DepuChef.Application.Models;

public class Ingredient
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public string? Category { get; set; }
    public List<string>? Items { get; set; }
}
