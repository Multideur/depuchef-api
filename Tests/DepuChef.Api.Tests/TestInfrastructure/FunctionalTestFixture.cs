using DepuChef.Infrastructure.DbContexts;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Testcontainers.MsSql;
using WireMock.Server;

namespace DepuChef.Api.FunctionalTests.TestInfrastructure;

public class FunctionalTestFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
        .WithPassword("mssql")
        .WithCleanUp(true)
        .Build();

    private HttpClient? _client;
    private WebApplicationFactory<Program>? _webApplicationFactory;

    private WebApplicationFactory<Program> WebApplicationFactory
    {
        get => _webApplicationFactory ?? throw new InvalidOperationException($"Cannot access {nameof(WebApplicationFactory)} before initialisation.");
        set => _webApplicationFactory = value;
    }

    public WireMockServer MockServer { get; }
    public const int WireMockPort = 8080;

    public IServiceProvider Services => WebApplicationFactory.Services;

    public HttpClient Client
    {
        get => _client ?? throw new InvalidOperationException($"Cannot access {nameof(DbContext)} before initialisation.");
        private set => _client = value;
    }

    public FunctionalTestFixture()
    {
        MockServer = WireMockServer.Start(WireMockPort);
    }

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();

        WebApplicationFactory = new TestApiApplication().WithWebHostBuilder(builder => builder.ConfigureTestServices(
            services =>
            {
                services
                    .RemoveType(typeof(DbContextOptions<DepuChefDbContext>))
                    .AddDbContext<DepuChefDbContext>(options =>
                        options
                            .UseSqlServer(_msSqlContainer.GetConnectionString())
                            .EnableSensitiveDataLogging()
                    );

                using var scope = services.BuildServiceProvider().CreateScope();
                var serviceProvider = scope.ServiceProvider;
            }));

        Client = WebApplicationFactory.CreateClient();

        var dbContext = WebApplicationFactory.Services.GetRequiredService<DepuChefDbContext>();
        while (!await dbContext.Database.CanConnectAsync())
        {
            await Task.Delay(10);
        }
        await dbContext.Database.MigrateAsync();
    }

    public Task DisposeAsync()
    {
        MockServer.Stop();
        MockServer.Dispose();
        return _msSqlContainer.DisposeAsync().AsTask();
    }
}