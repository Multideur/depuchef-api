using Microsoft.Extensions.DependencyInjection;

namespace DepuChef.Api.FunctionalTests;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RemoveType(this IServiceCollection services, Type type)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == type);
        if (descriptor != null)
            services.Remove(descriptor);

        return services;
    }
}