using DepuChef.Application.Models;
using DepuChef.Application.Services;
using DotnetGeminiSDK.Client.Interfaces;

namespace DepuChef.Provider.Gemini;

public class GeminiRecipeService(IGeminiClient geminiClient) : IRecipeService
{
    public async Task CreateRecipeFromImage(BackgroundRecipeRequest recipeRequest, CancellationToken cancellationToken)
    {

    }

    public Task<Recipe?> GetRecipeFromThread(string threadId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
