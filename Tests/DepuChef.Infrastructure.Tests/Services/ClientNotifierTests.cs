using DepuChef.Infrastructure.Hubs;
using DepuChef.Infrastructure.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;

namespace DepuChef.Infrastructure.Tests.Services;

public class ClientNotifierTests
{
    private readonly Mock<IHubContext<NotificationHub>> _mockHubContext = new();
    private readonly Mock<ISingleClientProxy> _mockClientProxy = new();
    private readonly Mock<ILogger<ClientNotifier>> _mockLogger = new();

    [Fact]
    public async Task NotifyRecipeReady_ShouldNotifyClient()
    {
        // Arrange
        var sut = CreateSut();
        var clientId = "clientId";
        var message = "message";
        var method = "RecipeReady";

        var mockHubClients = new Mock<IHubClients>();
;
        mockHubClients.Setup(x => x.Client(clientId))
            .Returns(_mockClientProxy.Object);

        _mockHubContext.Setup(x => x.Clients)
            .Returns(mockHubClients.Object);

        // Act
        await sut.NotifyRecipeReady(clientId, message, default);

        // Assert
        _mockClientProxy.Verify(x => x.SendCoreAsync(method, new object[1] { message }, default));
    }

    private ClientNotifier CreateSut() =>
        new(
            _mockHubContext.Object,
            _mockLogger.Object
            );
}
