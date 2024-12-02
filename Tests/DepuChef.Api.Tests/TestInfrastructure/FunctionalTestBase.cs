using AutoFixture;
using Xunit;

namespace DepuChef.Api.FunctionalTests.TestInfrastructure;

public abstract class FunctionalTestBase(FunctionalTestFixture fixture) : IAsyncLifetime
{
    protected readonly IFixture AutoFixture = new Fixture();
    protected readonly FunctionalTestFixture Fixture = fixture;

    protected FunctionalTestFixture Given => Fixture;
    protected FunctionalTestFixture WhenI => Fixture;
    protected FunctionalTestFixture Then => Fixture;
    protected HttpClient ResponseFor => Fixture.Client;

    public Task InitializeAsync()
    {
        //Fixture.StubAuthTokenResponse();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        Fixture.MockServer.Reset();
        return Fixture.ResetDatabase();
    }
}