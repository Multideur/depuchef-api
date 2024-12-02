using Microsoft.AspNetCore.SignalR;

namespace DepuChef.Infrastructure.Hubs;

public class NotificationHub : Hub
{
    public async Task NotifyRecipeReady(string clientId, string message) 
        => await Clients.Client(clientId).SendAsync("RecipeReady", message);

    public string GetConnectionId() => Context.ConnectionId;
}
