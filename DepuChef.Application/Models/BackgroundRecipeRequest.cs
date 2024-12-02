namespace DepuChef.Application.Models;

public class BackgroundRecipeRequest : RecipeRequest
{
    public MemoryStream? Stream { get; set; }
}
