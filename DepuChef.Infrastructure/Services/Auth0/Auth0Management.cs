using Auth0.ManagementApi;
using DepuChef.Application.Models.Auth;
using DepuChef.Application.Services;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using DepuChef.Application.Constants;

namespace DepuChef.Infrastructure.Services.Auth0;

public class Auth0Management(
    IOptions<AuthenticationOptions> options,
    IHttpService httpService,
    ILogger<Auth0Management> logger
    ) : IAuthManagementService
{
    private readonly AuthenticationOptions _options = options.Value;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented = true
    };

    public async Task DeleteUser(string authUserId, CancellationToken cancellationToken)
    {
        var tokenResponse = await GetToken(cancellationToken);
        var accessToken = tokenResponse?.AccessToken;
        if (string.IsNullOrEmpty(accessToken))
            throw new Exception("Failed to get access token.");

        var managementApi = new ManagementApiClient(accessToken, _options.Authority);
        
        logger.LogInformation($"Deleting auth user: {{{LogToken.AuthUserId}}}.", authUserId);

        await managementApi.Users.DeleteAsync(authUserId);
    }

    private async Task<TokenResponse?> GetToken(CancellationToken cancellationToken)
    {
        var request = new TokenRequest
        {
            ClientId = _options.ClientId,
            ClientSecret = _options.ClientSecret,
            Audience = _options.ManagementAudience,
            GrantType = "client_credentials"
        };
        var content = new StringContent(JsonSerializer.Serialize(request, _jsonSerializerOptions),
            Encoding.UTF8,
            "application/json");

        var response = await httpService.PostAsync(
            $"{_options.Authority}/oauth/token",
            content,
            cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Failed to get token: {ErrorContent}", errorContent);

            return null;
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        return JsonSerializer.Deserialize<TokenResponse>(responseContent, _jsonSerializerOptions);
    }
}
