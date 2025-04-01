namespace DepuChef.Api.Models;

public class RecipeResponse
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? Calories { get; internal set; }
    public int CaloriesAfterSubstitution { get; internal set; }
    public string? PrepTime { get; set; }
    public string? CookTime { get; set; }
    public string? TotalTime { get; set; }
    public int Servings { get; set; }
    public List<IngredientDto>? Ingredients { get; set; }
    public List<InstructionDto>? Instructions { get; set; }
    public List<NoteDto>? Notes { get; set; }
    public decimal Confidence { get; set; }
    public int? Rating { get; set; }
    public bool IsFavourite { get; set; }
}
