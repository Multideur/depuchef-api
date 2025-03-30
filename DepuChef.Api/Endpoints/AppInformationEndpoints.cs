using DepuChef.Application.Constants;

namespace DepuChef.Api.Endpoints;

public static class AppInformationEndpoints
{
    public static void MapInformationEndpoints(this WebApplication app)
    {
        app.MapGet("/app/information", () =>
        {
            var result = new AppInformation
            {
                ChefChoices = ChefChoice.List,
                SubscriptionLevels = SubscriptionLevel.List
            };
            return Results.Ok(result);
        }).AllowAnonymous();
    }
}

public class AppInformation
{
    public IReadOnlyCollection<ChefChoice> ChefChoices { get; set; } = [];
    public IReadOnlyCollection<SubscriptionLevel> SubscriptionLevels { get; set; } = [];
}
