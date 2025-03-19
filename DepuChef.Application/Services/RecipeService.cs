using DepuChef.Application.Models;
using DepuChef.Application.Repositories;

namespace DepuChef.Application.Services;

public class RecipeService(IRecipeRepository recipeRepository) : IRecipeService
{
    public async Task<IList<Recipe>> GetRecipes(Guid userId, CancellationToken cancellationToken = default) =>
        await recipeRepository.GetRecipes(r => r.UserId == userId, cancellationToken);

    public async Task<Recipe?> UpdateRecipeFavourite(Guid userId, Guid recipeId, bool isFavourite, CancellationToken cancellationToken)
    {
        var recipe = await recipeRepository.GetRecipe(recipeId, cancellationToken);
        if (recipe is null)
        {
            return null;
        }
        if (recipe.UserId != userId)
        {
            return null;
        }
        recipe.IsFavourite = isFavourite;
        return await recipeRepository.Update(recipe, cancellationToken);
    }
}
