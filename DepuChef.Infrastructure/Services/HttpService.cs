using DepuChef.Application.Models.OpenAI;
using DepuChef.Application.Services;
using Microsoft.Extensions.Options;

namespace DepuChef.Infrastructure.Services;

public class HttpService(IOptions<OpenAiOptions> options) : IHttpService
{
    private readonly OpenAiOptions _openAiOptions = options.Value;

    public async Task<HttpResponseMessage> GetAsync(string url,
        Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        using HttpClient client = CreateClient(headers);
        return await client.GetAsync(url, cancellationToken);
    }

    public async Task<HttpResponseMessage> PostAsync(string url,
        HttpContent content, 
        Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        using HttpClient client = CreateClient(headers);
        return await client.PostAsync(url, content, cancellationToken);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string url,
        Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        using HttpClient client = CreateClient(headers);
        return await client.DeleteAsync(url, cancellationToken);
    }

    private HttpClient CreateClient(Dictionary<string, string>? additionalHeaders)
    {
        var client = new HttpClient();
        var defaultHeaders = CreateHeaders();

        if (defaultHeaders != null)
            foreach (var header in defaultHeaders)
                client.DefaultRequestHeaders.Add(header.Key, header.Value);

        if (additionalHeaders != null)
            foreach (var header in additionalHeaders)
                client.DefaultRequestHeaders.Add(header.Key, header.Value);

        return client;
    }

    private Dictionary<string, string> CreateHeaders() =>
    new()
    {
        { "Authorization", $"Bearer {_openAiOptions.ApiKey}" },
        { "OpenAI-Beta", "assistants=v2" },
        { "Accept", "application/json" }
    };
}
