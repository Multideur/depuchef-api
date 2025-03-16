namespace DepuChef.Application.Models;

public class Ingredient
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public string? Category { get; set; }
    public int? Calories { get; set; }
    public List<IngredientItem>? Items { get; set; }
}

public class IngredientItem
{
    public Guid Id { get; set; }
    public Guid IngredientId { get; set; }
    public required string Name { get; set; }
    public int? Calories { get; set; }
}
