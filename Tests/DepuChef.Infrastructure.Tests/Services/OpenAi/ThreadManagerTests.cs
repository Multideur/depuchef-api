using DepuChef.Application.Models.OpenAI;
using DepuChef.Application.Models.OpenAI.Thread;
using DepuChef.Application.Services;
using DepuChef.Infrastructure.Services.OpenAi;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using System.Text.Json;

namespace DepuChef.Infrastructure.Tests.Services.OpenAi;

public class ThreadManagerTests
{
    private readonly Mock<IHttpService> _mockHttpService = new();
    private readonly Mock<IOptions<OpenAiOptions>> _mockOptions = new();
    private readonly Mock<ILogger<ThreadManager>> _mockLogger = new();

    public ThreadManagerTests()
    {
        _mockOptions.Setup(x => x.Value).Returns(new OpenAiOptions
        {
            ApiKey = "apiKey",
            AssistantId = "assistantId",
            BaseUrl = "http://test.com"
        });
    }

    [Fact]
    public async Task CreateThread_ReturnsThreadResponse()
    {
        const string assistantId = "assistantId";
        const string threadId = "threadId";
        var sut = CreateSut();
        var cancellationToken = new CancellationToken();
        var threadRequest = new ThreadRequest
        {
            AssistantId = assistantId
        };
        var imageFileReference = new ImageFileReference
        {
            FileId = "fileId"
        };
        var messageContent = new ContentItem
        {
            Type = "text",
            ImageFile = imageFileReference
        };
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(new ThreadResponse
            {
                Id = threadId
            })
                       )
        };

        _mockHttpService.Setup(x => x.PostAsync(
                It.IsAny<string>(), 
                It.IsAny<HttpContent>(), 
                null,
                cancellationToken))
            .ReturnsAsync(response);

        var result = await sut.CreateThread(threadRequest, cancellationToken);

        using var _ = new AssertionScope();
        result.Should().NotBeNull();
        result!.Id.Should().Be(threadId);
    }

    [Fact]
    public async Task CreateRun_ReturnsRunResponse()
    {
        const string threadId = "threadId";
        const string runId = "runId";
        var sut = CreateSut();
        var cancellationToken = new CancellationToken();
        var runRequest = new RunRequest
        {
            ThreadId = threadId
        };
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(new RunResponse
            {
                Id = runId
            }))
        };

        _mockHttpService.Setup(x => x.PostAsync(
                It.IsAny<string>(),
                It.IsAny<HttpContent>(),
                null,
                cancellationToken))
            .ReturnsAsync(response);

        var result = await sut.CreateRun(runRequest, cancellationToken);

        using var _ = new AssertionScope();
        result.Should().NotBeNull();
        result!.Id.Should().Be(runId);
    }

    [Fact]
    public async Task CreateThreadAndRun_ReturnsRunResponse()
    {
        const string runId = "runId";
        var sut = CreateSut();
        var cancellationToken = new CancellationToken();
        var threadRequest = new ThreadRequest
        {
            AssistantId = "assistantId"
        };
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(new RunResponse
            {
                Id = runId
            }))
        };

        _mockHttpService.Setup(x => x.PostAsync(
                It.IsAny<string>(),
                It.IsAny<HttpContent>(),
                null,
                cancellationToken))
            .ReturnsAsync(response);

        var result = await sut.CreateThreadAndRun(threadRequest, cancellationToken);

        using var _ = new AssertionScope();
        result.Should().NotBeNull();
        result!.Id.Should().Be(runId);
    }

    [Fact]
    public async Task CheckRunStatus_ReturnsRunResponse()
    {
        const string runId = "runId";
        const string threadId = "threadId";
        var sut = CreateSut();
        var cancellationToken = new CancellationToken();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(new RunResponse
            {
                Id = runId
            }))
        };

        _mockHttpService.Setup(x => x.GetAsync(
                It.IsAny<string>(),
                null,
                cancellationToken))
            .ReturnsAsync(response);

        var result = await sut.CheckRunStatus(threadId, runId, cancellationToken);

        using var _ = new AssertionScope();
        result.Should().NotBeNull();
        result!.Id.Should().Be(runId);
    }

    [Fact]
    public async Task DeleteThread_ReturnsDeleteThreadResponse()
    {
        const string threadId = "threadId";
        var sut = CreateSut();
        var cancellationToken = new CancellationToken();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(new DeleteThreadResponse
            {
                Id = threadId
            }))
        };

        _mockHttpService.Setup(x => x.DeleteAsync(
                It.IsAny<string>(),
                null,
                cancellationToken))
            .ReturnsAsync(response);

        var result = await sut.DeleteThread(threadId, cancellationToken);

        using var _ = new AssertionScope();
        result.Should().NotBeNull();
        result!.Id.Should().Be(threadId);
    }

    private ThreadManager CreateSut() =>
        new(
            _mockHttpService.Object,
            _mockOptions.Object,
            _mockLogger.Object
            );
}
