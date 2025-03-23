using DepuChef.Application.Models.OpenAI;
using DepuChef.Application.Models.OpenAI.File;
using DepuChef.Application.Services;
using DepuChef.Infrastructure.Services.OpenAi;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using System.Text.Json;

namespace DepuChef.Infrastructure.Tests.Services.OpenAi;

public class FileManagerTests
{
    private readonly Mock<IOptions<OpenAiOptions>> _mockOptions = new();
    private readonly Mock<ILogger<FileManager>> _mockLogger = new();
    private readonly Mock<IHttpService> _mockHttpService = new();

    public FileManagerTests()
    {
        _mockOptions.Setup(x => x.Value).Returns(new OpenAiOptions
        {
            ApiKey = "apiKey",
            AssistantId = "assistantId",
            BaseUrl = "http://test.com"
        });
    }

    [Fact]
    public async Task UploadFile_ShouldReturnFileUploadResponse()
    {
        var sut = CreateSut();
        var file = new FormFile(Stream.Null, 0, 0, "fileName", "fileName")
        {
            Headers = new HeaderDictionary()
            {
                { "Content-Type", "image/jpeg" }
            }
        };
        var fileUploadRequest = new FileUploadRequest
        {
            Purpose = "vision",
            File = file,
            Stream = new MemoryStream()
        };

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(new FileUploadResponse
            {
                Id = "fileId"
            }))
        };

        _mockHttpService
            .Setup(x => x.PostAsync(It.IsAny<string>(), 
                It.IsAny<HttpContent>(),
                null,
                default))
            .ReturnsAsync(response);

        var result = await sut.UploadFile(fileUploadRequest, default);

        using var _ = new AssertionScope();
        result.Should().NotBeNull();
        result!.Id.Should().Be("fileId");
    }

    [Fact]
    public async Task UploadFile_WhenResponseIsNotSuccessful_ShouldThrowException()
    {
        var sut = CreateSut();
        var file = new FormFile(Stream.Null, 0, 0, "fileName", "fileName");
        var fileUploadRequest = new FileUploadRequest
        {
            Purpose = "vision",
            File = file
        };

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest
        };

        _mockHttpService
            .Setup(x => x.PostAsync(It.IsAny<string>(),
                It.IsAny<HttpContent>(),
                null,
                default))
            .ReturnsAsync(response);

        Func<Task> act = async () => await sut.UploadFile(fileUploadRequest, default);

        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task DeleteFile_ShouldReturnFileDeleteResponse()
    {
        var sut = CreateSut();
        var fileId = "fileId";

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(new DeleteFileResponse
            {
                Id = fileId
            }))
        };

        _mockHttpService
            .Setup(x => x.DeleteAsync(It.IsAny<string>(), null, default))
            .ReturnsAsync(response);

        var result = await sut.DeleteFile(fileId, default);

        using var _ = new AssertionScope();
        result.Should().NotBeNull();
        result!.Id.Should().Be(fileId);
    }

    [Fact]
    public async Task DeleteFile_WhenResponseIsNotSuccessful_ShouldReturnNull()
    {
        var sut = CreateSut();
        var fileId = "fileId";

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest
        };

        _mockHttpService
            .Setup(x => x.DeleteAsync(It.IsAny<string>(), null, default))
            .ReturnsAsync(response);

        var result = await sut.DeleteFile(fileId, default);

        result.Should().BeNull();
    }

    private FileManager CreateSut()
    {
        return new FileManager(
            _mockHttpService.Object,
            _mockOptions.Object, 
            _mockLogger.Object
            );
    }
}
