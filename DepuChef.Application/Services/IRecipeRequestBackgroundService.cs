using DepuChef.Application.Models;

namespace DepuChef.Application.Services;

public interface IRecipeRequestBackgroundService
{
    void EnqueueRecipeRequest(BackgroundRecipeRequest recipeRequest);
}
