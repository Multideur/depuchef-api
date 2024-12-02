using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace DepuChef.Api.FunctionalTests.TestInfrastructure;

public class TestApiApplication : WebApplicationFactory<Program>
{
    private const string Environment = "Testing";

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(Environment);

        builder.ConfigureServices(_ =>
        {
        });

        return base.CreateHost(builder);
    }
}