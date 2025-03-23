using DepuChef.Application.Models;

namespace DepuChef.Application.Services;

public interface IRecipeService
{
    Task<IList<Recipe>> GetRecipesForUser(Guid userId, CancellationToken cancellationToken);
    Task<Recipe?> UpdateRecipeFavourite(Guid userId, Guid recipeId, bool isFavourite, CancellationToken cancellationToken);
}
