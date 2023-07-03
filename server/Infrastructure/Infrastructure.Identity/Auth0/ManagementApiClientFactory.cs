using System.Text.Json;
using System.Text.Json.Serialization;
using Auth0.ManagementApi;
using Microsoft.Extensions.Options;

namespace Infrastructure.Identity.Auth0;

internal sealed class ManagementApiClientFactory
{
    private readonly Auth0ManagementApiOptions _options;
    private readonly HttpClient _httpClient;
    private static string? Token { get; set; }
    private static DateTime? TokenExpires { get; set; }

    public ManagementApiClientFactory(
        HttpClient httpClient,
        IOptions<Auth0ManagementApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<ManagementApiClient> GetClientAsync(CancellationToken cancellationToken = default)
    {
        return new ManagementApiClient(await GetAccessTokenAsync(cancellationToken), _options.Domain);
    }

    private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (Token is null || TokenExpires is null || TokenExpires < DateTime.UtcNow) // No need for a change to this check as we set the expiry lower below
        {
            // Make authentication request to Auth0 to get a management API token
            var content = new FormUrlEncodedContent(new []
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", _options.ClientId),
                new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
                new KeyValuePair<string, string>("audience", $"https://{_options.Domain}/api/v2/"),
            });
            var response = await _httpClient.PostAsync($"https://{_options.Domain}/oauth/token", content, cancellationToken);
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var tokenData = JsonSerializer.Deserialize<Auth0ManagementApiTokenResponse>(json);

            Token = tokenData.AccessToken;
            TokenExpires = DateTime.UtcNow.AddHours(23).AddMinutes(30); // Actually expires in 24 hours but lets refresh a little early
        }

        return Token;
    }

    private struct Auth0ManagementApiTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}
