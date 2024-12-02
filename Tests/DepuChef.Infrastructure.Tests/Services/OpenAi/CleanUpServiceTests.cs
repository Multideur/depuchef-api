using DepuChef.Application.Models.OpenAI;
using DepuChef.Application.Models.OpenAI.CleanUp;
using DepuChef.Application.Models.OpenAI.File;
using DepuChef.Application.Models.OpenAI.Thread;
using DepuChef.Application.Services.OpenAi;
using DepuChef.Infrastructure.Services.OpenAi;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace DepuChef.Infrastructure.Tests.Services.OpenAi;

public class CleanUpServiceTests
{
    private readonly Mock<IFileManager> _mockFileManager = new();
    private readonly Mock<IThreadManager> _mockThreadManager = new();
    private readonly Mock<IOptions<OpenAiOptions>> _mockOptions = new();
    private readonly Mock<ILogger<CleanUpService>> _mockLogger = new();

    public CleanUpServiceTests()
    {
        _mockOptions.Setup(x => x.Value).Returns(new OpenAiOptions
        {
            ApiKey = "apiKey",
            AssistantId = "assistantId",
            BaseUrl = "http://test.com"
        });
    }

    [Fact]
    public async Task CleanUp_ShouldDeleteFile()
    {
        // Arrange
        var sut = CreateSut();
        var fileId = "fileId";
        var threadId = "threadId";
        var cleanUpRequest = new CleanUpRequest
        {
            FileId = fileId,
            ThreadId = threadId
        };

        _mockFileManager.Setup(x => x.DeleteFile(fileId, default))
            .ReturnsAsync(new DeleteFileResponse
            {
                Deleted = true
            });

        // Act
        await sut.CleanUp(cleanUpRequest, default);

        // Assert
        _mockFileManager.Verify(x =>  x.DeleteFile(fileId, default));
    }

    [Fact]
    public async Task CleanUp_ShouldDeleteThread()
    {
        // Arrange
        var sut = CreateSut();
        var fileId = "fileId";
        var threadId = "threadId";
        var cleanUpRequest = new CleanUpRequest
        {
            FileId = fileId,
            ThreadId = threadId
        };

        _mockFileManager.Setup(x => x.DeleteFile(fileId, default))
            .ReturnsAsync(new DeleteFileResponse
            {
                Deleted = true
            });

        _mockThreadManager.Setup(x => x.DeleteThread(threadId, default))
            .ReturnsAsync(new DeleteThreadResponse
            {
                Deleted = true
            });

        // Act
        await sut.CleanUp(cleanUpRequest, default);

        // Assert
        _mockThreadManager.Verify(x => x.DeleteThread(threadId, default));
    }

    private CleanUpService CreateSut() =>
        new (
            _mockFileManager.Object,
            _mockThreadManager.Object,
            _mockOptions.Object,
            _mockLogger.Object
            );
}
