using DepuChef.Application.Models;
using DepuChef.Application.Models.OpenAI;
using DepuChef.Application.Models.OpenAI.CleanUp;
using DepuChef.Application.Models.OpenAI.File;
using DepuChef.Application.Models.OpenAI.Thread;
using DepuChef.Application.Repositories;
using DepuChef.Application.Services;
using DepuChef.Application.Services.OpenAi;
using DepuChef.Infrastructure.Services.OpenAi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace DepuChef.Infrastructure.Tests.Services.OpenAi;

public class OpenAiRecipeServiceTests
{
    private const string AssistantId = "assistantId";
    private readonly Mock<IFileManager> _mockFileManager = new();
    private readonly Mock<IThreadManager> _mockThreadManager = new();
    private readonly Mock<IMessageManager> _mockMessageManager = new();
    private readonly Mock<ICleanUpService> _mockCleanUpService = new();
    private readonly Mock<IClientNotifier> _mockClientNotifier = new();
    private readonly Mock<IProcessRepository> _mockProcessRepository = new();
    private readonly Mock<IOptions<OpenAiOptions>> _mockOptions = new();
    private readonly Mock<ILogger<OpenAiRecipeService>> _mockLogger = new();

    public OpenAiRecipeServiceTests()
    {
        _mockOptions.Setup(x => x.Value).Returns(new OpenAiOptions
        {
            ApiKey = "apiKey",
            AssistantId = "assistantId",
            BaseUrl = "http://test.com"
        });
    }

    [Fact]
    public async Task CreateRecipeFromImage_ShouldUploadFile()
    {
        // Arrange
        var sut = CreateSut();
        var recipeRequest = new BackgroundRecipeRequest
        {
            Image = new FormFile(Stream.Null, 0, 0, "fileName", "fileName"),
            Stream = new MemoryStream(),
            ConnectionId = "connectionId"
        };

        // Act
        await sut.CreateRecipeFromImage(recipeRequest);

        // Assert
        _mockFileManager.Verify(x =>
            x.UploadFile(
                It.Is<FileUploadRequest>(request =>
                    request.Purpose == "vision"
                    && request.File!.FileName == "fileName"),
                default));
    }

    [Fact]
    public async Task CreateRecipeFromImage_WhenFileUploadResponseIdIsNotNull_ShouldCreateThreadRequest()
    {
        // Arrange
        var sut = CreateSut();
        var recipeRequest = new BackgroundRecipeRequest
        {
            Image = new FormFile(Stream.Null, 0, 0, "fileName", "fileName"),
            Stream = new MemoryStream(),
            ConnectionId = "connectionId"
        };
        _mockFileManager.Setup(x => x.UploadFile(It.IsAny<FileUploadRequest>(), default))
            .ReturnsAsync(new FileUploadResponse { Id = "fileId" });

        // Act
        await sut.CreateRecipeFromImage(recipeRequest);

        // Assert
        _mockThreadManager.Verify(x =>
            x.CreateThreadAndRun(
                It.Is<ThreadRequest>(request =>
                    request.AssistantId == AssistantId
                    && request.Thread!.Messages![0].Content![0].ImageFile!.FileId == "fileId"
                    && request.Thread.Messages[0].Role == "user"
                ),
                default));
    }

    [Fact]
    public async Task CreateRecipeFromImage_WhenRunStatusCompleted_ShouldNotifyClient()
    {
        // Arrange
        var sut = CreateSut();
        const string connectionId = "client Id";
        const string threadId = "threadId";

        var recipeRequest = new BackgroundRecipeRequest
        {
            Image = new FormFile(Stream.Null, 0, 0, "fileName", "fileName"),
            Stream = new MemoryStream(),
            ConnectionId = connectionId
        };

        _mockFileManager.Setup(x => x.UploadFile(It.IsAny<FileUploadRequest>(), default))
            .ReturnsAsync(new FileUploadResponse { Id = "fileId" });

        _mockThreadManager.Setup(x => x.CreateThreadAndRun(It.IsAny<ThreadRequest>(), default))
            .ReturnsAsync(new RunResponse { ThreadId = threadId, Id = "id" });

        _mockThreadManager.Setup(x => x.CheckRunStatus(threadId, "id", default))
            .ReturnsAsync(new RunResponse { Status = "completed" });

        // Act
        await sut.CreateRecipeFromImage(recipeRequest);

        // Assert
        _mockClientNotifier.Verify(x => x.NotifyRecipeReady(connectionId, threadId, default));
    }

    [Fact]
    public async Task CreateRecipeFromImage_WhenRunStatusCompleted_ShouldCleanUp()
    {
        // Arrange
        var sut = CreateSut();
        const string connectionId = "client Id";
        const string threadId = "threadId";
        const string fileId = "fileId";

        var recipeRequest = new BackgroundRecipeRequest
        {
            Image = new FormFile(Stream.Null, 0, 0, "fileName", "fileName"),
            Stream = new MemoryStream(),
            ConnectionId = connectionId
        };

        _mockFileManager.Setup(x => x.UploadFile(It.IsAny<FileUploadRequest>(), default))
            .ReturnsAsync(new FileUploadResponse { Id = fileId });

        _mockThreadManager.Setup(x => x.CreateThreadAndRun(It.IsAny<ThreadRequest>(), default))
            .ReturnsAsync(new RunResponse { ThreadId = threadId, Id = "id" });

        _mockThreadManager.Setup(x => x.CheckRunStatus(threadId, "id", default))
            .ReturnsAsync(new RunResponse { Status = "completed" });

        // Act
        await sut.CreateRecipeFromImage(recipeRequest);

        // Assert
        _mockCleanUpService.Verify(x => x.CleanUp(It.Is<CleanUpRequest>(request =>
                request.FileId == fileId
                && request.ThreadId == threadId),
            default));
    }

    private OpenAiRecipeService CreateSut() =>
        new(
            _mockFileManager.Object,
            _mockThreadManager.Object,
            _mockMessageManager.Object,
            _mockCleanUpService.Object,
            _mockClientNotifier.Object,
            _mockProcessRepository.Object,
            _mockOptions.Object,
            _mockLogger.Object
            );
}
