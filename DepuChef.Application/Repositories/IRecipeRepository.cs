using DepuChef.Application.Models;

namespace DepuChef.Application.Repositories;

public interface IRecipeRepository
{
    Task Add(Recipe recipe);
}
