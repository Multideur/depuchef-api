using DepuChef.Application.Models;

namespace DepuChef.Application.Services.OpenAi;

public interface IAiRecipeService
{
    Task CreateRecipeFromImage(BackgroundRecipeRequest recipeRequest, CancellationToken cancellationToken);
    Task<Recipe?> GetRecipeByProcessId(Guid processId, CancellationToken cancellationToken);
}