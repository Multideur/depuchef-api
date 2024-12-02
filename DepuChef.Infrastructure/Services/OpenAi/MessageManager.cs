using DepuChef.Application.Models.OpenAI;
using DepuChef.Application.Models.OpenAI.Message;
using DepuChef.Application.Services;
using DepuChef.Application.Services.OpenAi;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace DepuChef.Infrastructure.Services.OpenAi;

public class MessageManager(IHttpService httpService,
    IOptions<OpenAiOptions> options,
    ILogger<MessageManager> logger) : BaseOpenAiService(options), IMessageManager
{
    public async Task<List<Message>?> GetMessages(string threadId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting messages for thread: {threadId}", threadId);
        var url = $"{_openAiOptions.BaseUrl}/threads/{threadId}/messages";
        var response = await httpService.GetAsync(url,
            cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to get messages");

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var messages = JsonSerializer.Deserialize<GetMessagesResponse>(responseContent);

        return messages?.Data;
    }
}
