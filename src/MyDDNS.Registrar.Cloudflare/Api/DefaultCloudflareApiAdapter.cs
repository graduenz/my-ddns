using System.Net.Http.Headers;
using System.Net.Http.Json;
using MyDDNS.Registrar.Cloudflare.Api.Requests;
using MyDDNS.Registrar.Cloudflare.Api.Responses;

namespace MyDDNS.Registrar.Cloudflare.Api;

public class DefaultCloudflareApiAdapter : ICloudflareApiAdapter
{
    private const string CloudflareApiAddress = "https://api.cloudflare.com/client/v4/";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _authEmail;
    private readonly string _authToken;

    private HttpClient? _httpClient;

    public DefaultCloudflareApiAdapter(IHttpClientFactory httpClientFactory, string authEmail, string authToken)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _authEmail = authEmail ?? throw new ArgumentNullException(nameof(authEmail));
        _authToken = authToken ?? throw new ArgumentNullException(nameof(authToken));
    }

    public async Task<GetDnsRecordsResponse?> GetDnsRecordsAsync(string zoneIdentifier, string recordName,
        CancellationToken cancellationToken = default)
    {
        var requestUri = $"{zoneIdentifier}/dns_records?type=A&name={recordName}";

        using var httpClient = GetCloudflareApiHttpClient();
        var response = await httpClient.GetAsync(requestUri, cancellationToken);

        EnsureHttpStatusSuccess(response);

        return await response.Content.ReadFromJsonAsync<GetDnsRecordsResponse>(cancellationToken);
    }

    public async Task<PatchDnsRecordResponse?> PatchDnsRecordAsync(string zoneIdentifier, string recordId,
        PatchDnsRecordRequest payload, CancellationToken cancellationToken = default)
    {
        var requestUri = $"{zoneIdentifier}/dns_records/{recordId}";

        using var httpClient = GetCloudflareApiHttpClient();
        var response = await httpClient.PatchAsJsonAsync(requestUri, payload, cancellationToken);

        EnsureHttpStatusSuccess(response);

        return await response.Content.ReadFromJsonAsync<PatchDnsRecordResponse>(cancellationToken);
    }

    private static void EnsureHttpStatusSuccess(HttpResponseMessage? response)
    {
        if (response is not { IsSuccessStatusCode: true })
            throw new InvalidOperationException(
                $"Expected Cloudflare API response to have a success status code, but got {response?.StatusCode}.");
    }

    private HttpClient GetCloudflareApiHttpClient()
    {
        // ReSharper disable once InvertIf
        if (_httpClient == null)
        {
            _httpClient = _httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(CloudflareApiAddress);

            _httpClient.DefaultRequestHeaders.Add("X-Auth-Email", _authEmail);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
        }

        return _httpClient;
    }
}