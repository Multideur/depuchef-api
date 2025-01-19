using DepuChef.Application.Repositories;
using DepuChef.Infrastructure.DbContexts;
using DepuChef.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DepuChef.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureDatabase(
        this IServiceCollection services,
        string connectionString
    ) => services
            .AddDbContext<DepuChefDbContext>(options =>
                options.UseSqlServer(connectionString)
            )
            .AddRepositories();

    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IProcessRepository, ProcessRepository>();
}