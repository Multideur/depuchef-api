using DepuChef.Application.Models.OpenAI;
using DepuChef.Application.Models.OpenAI.Thread;
using DepuChef.Application.Services;
using DepuChef.Application.Services.OpenAi;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DepuChef.Infrastructure.Services.OpenAi;

public class ThreadManager(IHttpService httpService,
    IOptions<OpenAiOptions> options,
    ILogger<ThreadManager> logger) : BaseOpenAiService(options), IThreadManager
{
    public async Task<ThreadResponse?> CreateThread(ThreadRequest threadRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating thread for assistant: {assistantId}", threadRequest.AssistantId);

        var content = new StringContent(JsonSerializer.Serialize(threadRequest, _jsonSerializerOptions),
        Encoding.UTF8,
            "application/json");
        var threadUrl = $"{_openAiOptions.BaseUrl}/threads";
        var response = await httpService.PostAsync(threadUrl, 
            content,
            cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to create thread");

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ThreadResponse>(responseContent);
    }

    public async Task<RunResponse?> CreateRun(RunRequest runRequest, CancellationToken cancellationToken)
    {
        var runUrl = $"{_openAiOptions.BaseUrl}/threads/{runRequest.ThreadId}/runs";
        var runData = new
        {
            assistant_id = _openAiOptions.AssistantId,
        };
        var content = new StringContent(JsonSerializer.Serialize(runData, _jsonSerializerOptions),
        Encoding.UTF8,
            "application/json");
        var response = await httpService.PostAsync(runUrl, 
            content,
            cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to create run");

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var runResponse = JsonSerializer.Deserialize<RunResponse>(responseContent);

        return runResponse;
    }

    public async Task<RunResponse?> CreateThreadAndRun(ThreadRequest threadRequest, CancellationToken cancellationToken)
    {
        var url = $"{_openAiOptions.BaseUrl}/threads/runs";
        var content = new StringContent(JsonSerializer.Serialize(threadRequest, _jsonSerializerOptions),
        Encoding.UTF8,
            "application/json");
        var response = await httpService.PostAsync(url, 
            content,
            cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Failed to create thread and run. \nStatus code: {response.StatusCode}.\nUrl: {url} \nContent: {JsonSerializer.Serialize(content)}\nRequest: {threadRequest}");

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<RunResponse>(responseContent);
    }

    public async Task<RunResponse?> CheckRunStatus(string threadId, string runId, CancellationToken cancellationToken)
    {
        var threadUrl = $"{_openAiOptions.BaseUrl}/threads/{threadId}/runs/{runId}";

        var response = await httpService.GetAsync(threadUrl,
            cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to check status");

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<RunResponse>(responseContent);
    }

    public async Task<DeleteThreadResponse?> DeleteThread(string threadId, CancellationToken cancellationToken)
    {
        var threadUrl = $"{_openAiOptions.BaseUrl}/threads/{threadId}";

        var response = await httpService.DeleteAsync(threadUrl,
                       cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to delete thread");

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<DeleteThreadResponse>(responseContent);
    }
}
