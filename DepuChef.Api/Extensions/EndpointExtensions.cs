using DepuChef.Api.Endpoints;

namespace DepuChef.Api.Extensions;

public static class EndpointExtensions
{
    public static void AddEndpoints(this WebApplication app)
    {
        app.MapRecipeEndpoints();
        app.MapUserEndpoints();
        app.MapAdminEndpoints();
        app.MapInformationEndpoints();
    }
}