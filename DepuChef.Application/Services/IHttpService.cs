namespace DepuChef.Application.Services;

public interface IHttpService
{
    Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> PostAsync(string url, HttpContent content, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> DeleteAsync(string url, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default);
}