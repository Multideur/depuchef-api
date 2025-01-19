using DepuChef.Application.Models;

namespace DepuChef.Application.Services
{
    public interface IRecipeService
    {
        Task CreateRecipeFromImage(BackgroundRecipeRequest recipeRequest, CancellationToken cancellationToken);
        Task<Recipe?> GetRecipeByProcessId(Guid processId, CancellationToken cancellationToken); 
    }
}