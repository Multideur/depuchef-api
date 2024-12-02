namespace DepuChef.Application.Services;

public interface IClientNotifier
{
    Task NotifyRecipeReady(string clientId, string message, CancellationToken cancellationToken);
}
