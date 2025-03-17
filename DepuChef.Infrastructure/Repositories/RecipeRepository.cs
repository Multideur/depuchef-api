using DepuChef.Application.Models;
using DepuChef.Application.Repositories;
using DepuChef.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DepuChef.Infrastructure.Repositories;

public class RecipeRepository(DepuChefDbContext dbContext) : IRecipeRepository
{
    public async Task<Recipe> Add(Recipe recipe, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Recipes.AddAsync(recipe, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Entity;
    }

    public async Task<List<Recipe>> GetRecipes(Guid userId, CancellationToken cancellationToken) =>
        await dbContext.Recipes.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
}
