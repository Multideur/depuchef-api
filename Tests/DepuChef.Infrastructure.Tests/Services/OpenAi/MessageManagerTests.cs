using DepuChef.Application.Models.OpenAI;
using DepuChef.Application.Models.OpenAI.Message;
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

public class MessageManagerTests
{
    private readonly Mock<IHttpService> _mockHttpService = new();
    private readonly Mock<IOptions<OpenAiOptions>> _mockOptions = new();
    private readonly Mock<ILogger<MessageManager>> _mockLogger = new();

    public MessageManagerTests()
    {
        _mockOptions.Setup(x => x.Value).Returns(new OpenAiOptions
        {
            ApiKey = "apiKey",
            AssistantId = "assistantId",
            BaseUrl = "http://test.com"
        });
    }

    [Fact]
    public async Task GetMessages_ShouldReturnMessages()
    {
        const string messageId = "messageId";
        const string threadId = "threadId";
        var sut = CreateSut();
        var cancellationToken = new CancellationToken();
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
            Content = new StringContent(JsonSerializer.Serialize(new GetMessagesResponse
            {
                Data =
                [
                    new Message
                    {
                        Id = messageId,
                        ThreadId = threadId,
                        Content = new ContentItem[]
                        {
                            messageContent
                        },
                        CreatedAt = 12345
                    }
                ]
            }))
        };

        _mockHttpService.Setup(x => x.GetAsync(It.IsAny<string>(), null, cancellationToken))
            .ReturnsAsync(response);

        var messages = await sut.GetMessages(threadId, cancellationToken);

        using var _ = new AssertionScope();
        messages.Should().NotBeNull();
        messages.Should().HaveCount(1);
        var message = messages![0];
        message.Id.Should().Be(messageId);
        message.ThreadId.Should().Be(threadId);
        var content = message.Content as ContentItem[];
        content.Should().AllBeEquivalentTo(messageContent);
    }

    private MessageManager CreateSut() =>
        new(
            _mockHttpService.Object,
            _mockOptions.Object,
            _mockLogger.Object
            );
}
