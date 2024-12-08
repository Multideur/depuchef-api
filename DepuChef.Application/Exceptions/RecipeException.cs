using DepuChef.Application.Models;

namespace DepuChef.Application.Exceptions;

public class RecipeException(RecipeError recipeError) : Exception(recipeError.Message)
{
}
