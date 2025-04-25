namespace DepuChef.Application.Models;

public class Note
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public string? Text { get; set; }
}
