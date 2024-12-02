using Microsoft.Extensions.DependencyInjection;

namespace DepuChef.Api.FunctionalTests.TestInfrastructure;

internal static class FunctionalTestFixtureExtensions
{
    public static async Task<TResult> ExecuteAsync<TService, TResult>(this FunctionalTestFixture fixture, Func<TService, Task<TResult>> command) where TService : notnull
    {
        using var scope = fixture.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<TService>();
        return await command(service);
    }

    public static async Task ExecuteAsync<TService>(this FunctionalTestFixture fixture, Func<TService, Task> command) where TService : notnull
    {
        using var scope = fixture.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<TService>();
        await command(service);
    }
}