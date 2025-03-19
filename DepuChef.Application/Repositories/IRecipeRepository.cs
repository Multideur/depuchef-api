using DepuChef.Application.Models;
using System.Linq.Expressions;

namespace DepuChef.Application.Repositories;

public interface IRecipeRepository
{
    Task<Recipe> Add(Recipe recipe, CancellationToken cancellationToken);
    Task<Recipe?> GetRecipe(Guid recipeId, CancellationToken cancellationToken);
    Task<List<Recipe>> GetRecipes(Expression<Func<Recipe, bool>> expression, CancellationToken cancellationToken);
    Task<Recipe?> Update(Recipe recipe, CancellationToken cancellationToken);
}
