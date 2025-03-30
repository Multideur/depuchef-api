using DepuChef.Application.Constants;
using System.Collections.ObjectModel;

namespace DepuChef.Api.Endpoints;

public static class AppInformationEndpoints
{
    public static void MapAppInformationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/app/information", () =>
        {
            var result = new AppInformation
            {
                ChefChoices = new ReadOnlyCollection<ChefChoice>([.. ChefChoice.List.Order()]),
                SubscriptionLevels = new ReadOnlyCollection<SubscriptionLevel>([.. SubscriptionLevel.List.Order()])
            };
            return Results.Ok(result);
        })
            .WithName("GetAppInformation")
            .Produces<AppInformation>()
            .AllowAnonymous();
    }
}

public class AppInformation
{
    public IReadOnlyCollection<ChefChoice> ChefChoices { get; set; } = [];
    public IReadOnlyCollection<SubscriptionLevel> SubscriptionLevels { get; set; } = [];
}
