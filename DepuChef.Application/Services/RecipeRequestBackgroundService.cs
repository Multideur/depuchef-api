using DepuChef.Application.Models;
using DepuChef.Application.Services.OpenAi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace DepuChef.Application.Services;

public class RecipeRequestBackgroundService(
    IServiceProvider serviceProvider,
    Channel<BackgroundRecipeRequest> recipeRequestChannel,
    ILogger<RecipeRequestBackgroundService> logger
    ) : BackgroundService, IRecipeRequestBackgroundService
{
    public void EnqueueRecipeRequest(BackgroundRecipeRequest recipeRequest)
    {
        logger.LogInformation("Enqueued recipe request with ConnectionId: {connectionId}", recipeRequest.ConnectionId);
        recipeRequestChannel.Writer.TryWrite(recipeRequest);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await StartChannelListener(stoppingToken);
    }

    public async Task StartChannelListener(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await foreach (var recipeRequest in recipeRequestChannel.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    logger.LogInformation("Processing recipe request with ConnectionId: {connectionId}", recipeRequest.ConnectionId);

                    using var scope = serviceProvider.CreateScope();
                    var recipeService = scope.ServiceProvider.GetRequiredService<IAiRecipeService>();

                    if (recipeRequest.Image != null)
                    {
                        await recipeService.CreateRecipeFromImage(recipeRequest, stoppingToken);
                        return;
                    }

                    if (recipeRequest.Text != null)
                    {
                        await recipeService.CreateRecipeFromText(recipeRequest, stoppingToken); 
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing recipe request with ConnectionId: {connectionId}", recipeRequest.ConnectionId);
                    continue;
                }
            }
        }
    }
}
