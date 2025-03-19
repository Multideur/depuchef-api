using DepuChef.Application.Models;
using DepuChef.Application.Repositories;
using DepuChef.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DepuChef.Infrastructure.Repositories;

public class ProcessRepository(DepuChefDbContext dbContext) : IProcessRepository
{
    public async Task<RecipeProcess?> GetRecipeProcess(
        Expression<Func<RecipeProcess, bool>> predicate, 
        CancellationToken cancellationToken = default) =>
        await dbContext.RecipeProcesses.SingleOrDefaultAsync(predicate, cancellationToken);

    public async Task<RecipeProcess?> SaveRecipeProcess(RecipeProcess recipeProcess, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.RecipeProcesses.AddAsync(recipeProcess);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Entity;
    }

    public async Task DeleteRecipeProcess(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.RecipeProcesses.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            dbContext.RecipeProcesses.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
