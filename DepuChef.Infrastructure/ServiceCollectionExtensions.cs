using DepuChef.Application.Repositories;
using DepuChef.Infrastructure.DbContexts;
using DepuChef.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DepuChef.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureDatabase(
        this IServiceCollection services,
        string connectionString
    ) => services
            .AddDbContext<DepuChefDbContext>(optionsBuilder =>
                optionsBuilder.UseSqlServer(
                    connectionString,
                    options => options.EnableRetryOnFailure())
            )
            .AddRepositories();

    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services
            .AddScoped<IRecipeRepository, RecipeRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IProcessRepository, ProcessRepository>();
}