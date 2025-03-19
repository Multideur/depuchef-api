using DepuChef.Application.Models;
using System.Linq.Expressions;

namespace DepuChef.Application.Repositories;

public interface IProcessRepository
{
    Task<RecipeProcess?> GetRecipeProcess(Expression<Func<RecipeProcess, bool>> predicate, CancellationToken cancellationToken = default);
    Task<RecipeProcess?> SaveRecipeProcess(RecipeProcess recipeProcess, CancellationToken cancellationToken = default);
    Task DeleteRecipeProcess(Guid id, CancellationToken cancellationToken = default);
}
