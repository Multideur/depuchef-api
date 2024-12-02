using DepuChef.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DepuChef.Api.FunctionalTests.TestInfrastructure;

internal static class FunctionalTestFixtureDatabaseExtensions
{
    public static Task ResetDatabase(this FunctionalTestFixture fixture) => 
        fixture.ExecuteAsync<DepuChefDbContext>(dbContext => RelationalDatabaseFacadeExtensions.ExecuteSqlRawAsync(dbContext.Database, @"
            TRUNCATE ""Recipes"",""Ingredients"",""Users"";
            "));

    public static Task Existing<T>(this FunctionalTestFixture fixture, params T[] entities) where T : class
    {
        return fixture.ExecuteAsync<DepuChefDbContext>(async d =>
        {
            foreach (var entity in entities)
            {
                await d.AddAsync(entity);
            }
            await d.SaveChangesAsync();
        });
    }
}