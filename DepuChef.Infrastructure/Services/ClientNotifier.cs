using DepuChef.Application.Services;
using DepuChef.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DepuChef.Infrastructure.Services;

public class ClientNotifier(IHubContext<NotificationHub> hubContext,
    ILogger<ClientNotifier> logger) : IClientNotifier
{
    public async Task NotifyRecipeReady(string clientId, string message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Notifying client {clientId} that recipe is ready", clientId);
        await hubContext.Clients.Client(clientId).SendAsync("RecipeReady", 
            message, 
            cancellationToken: cancellationToken);
    }
}
