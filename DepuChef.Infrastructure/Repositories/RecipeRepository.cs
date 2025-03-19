using DepuChef.Application.Models;
using DepuChef.Application.Repositories;
using DepuChef.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DepuChef.Infrastructure.Repositories;

public class RecipeRepository(DepuChefDbContext dbContext) : IRecipeRepository
{
    public async Task<Recipe> Add(Recipe recipe, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Recipes.AddAsync(recipe, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Entity;
    }

    public async Task<Recipe?> GetRecipe(Guid recipeId, CancellationToken cancellationToken) =>
        await dbContext.Recipes.SingleOrDefaultAsync(r => r.Id == recipeId, cancellationToken);

    public async Task<List<Recipe>> GetRecipes(Expression<Func<Recipe, bool>> predicate, CancellationToken cancellationToken) =>
        await dbContext.Recipes.Where(predicate).ToListAsync(cancellationToken);

    public async Task<Recipe?> Update(Recipe recipe, CancellationToken cancellationToken)
    {
        var entity = dbContext.Recipes.Update(recipe);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Entity;
    }
}
