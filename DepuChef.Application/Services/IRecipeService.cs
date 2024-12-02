using DepuChef.Application.Models;

namespace DepuChef.Application.Services
{
    public interface IRecipeService
    {
        Task CreateRecipeFromImage(BackgroundRecipeRequest recipeRequest, CancellationToken cancellationToken);
        Task<Recipe?> GetRecipeFromThread(string threadId, CancellationToken cancellationToken); 
    }
}