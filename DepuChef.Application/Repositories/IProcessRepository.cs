using DepuChef.Application.Models;

namespace DepuChef.Application.Repositories;

public interface IProcessRepository
{
    Task<RecipeProcess?> GetRecipeProcessById(Guid id, CancellationToken cancellationToken = default);
    Task<RecipeProcess?> GetRecipeProcessByThreadId(string threadId, CancellationToken cancellationToken = default);
    Task<RecipeProcess?> SaveRecipeProcess(RecipeProcess recipeProcess, CancellationToken cancellationToken = default);
    Task DeleteRecipeProcess(Guid id, CancellationToken cancellationToken = default);
}
