using DepuChef.Application.Models;

namespace DepuChef.Application.Repositories;

public interface IRecipeRepository
{
    Task<Recipe> Add(Recipe recipe, CancellationToken cancellationToken);
    Task<List<Recipe>> GetRecipes(Guid userId, CancellationToken cancellationToken);
}
