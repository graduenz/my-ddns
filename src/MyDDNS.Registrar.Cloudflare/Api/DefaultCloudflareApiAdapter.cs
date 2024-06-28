using System.Net.Http.Headers;
using System.Net.Http.Json;
using MyDDNS.Registrar.Cloudflare.Api.Requests;
using MyDDNS.Registrar.Cloudflare.Api.Responses;

namespace MyDDNS.Registrar.Cloudflare.Api;

/// <summary>
/// Default implementation of <see cref="ICloudflareApiAdapter"/>.
/// </summary>
public class DefaultCloudflareApiAdapter : ICloudflareApiAdapter
{
    private const string CloudflareApiAddress = "https://api.cloudflare.com/client/v4/";

    private readonly IHttpClientFactory _httpClientFactory;

    private HttpClient? _httpClient;

    /// <summary>
    /// Creates the <see cref="DefaultCloudflareApiAdapter"/>.
    /// </summary>
    /// <param name="httpClientFactory">An instance of <see cref="IHttpClientFactory"/>.</param>
    /// <exception cref="ArgumentNullException">When any of the parameters are null.</exception>
    public DefaultCloudflareApiAdapter(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    /// <inheritdoc />
    public async Task<GetDnsRecordsResponse?> GetDnsRecordsAsync(
        string apiToken,
        string zoneIdentifier,
        string recordName,
        CancellationToken cancellationToken = default)
    {
        var requestUri = $"{zoneIdentifier}/dns_records?type=A&name={recordName}";

        using var httpClient = GetCloudflareApiHttpClient(apiToken);
        var response = await httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<GetDnsRecordsResponse>(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PatchDnsRecordResponse?> PatchDnsRecordAsync(
        string apiToken,
        string zoneIdentifier,
        string recordId,
        PatchDnsRecordRequest payload,
        CancellationToken cancellationToken = default)
    {
        var requestUri = $"{zoneIdentifier}/dns_records/{recordId}";

        using var httpClient = GetCloudflareApiHttpClient(apiToken);
        var response = await httpClient.PatchAsJsonAsync(requestUri, payload, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PatchDnsRecordResponse>(cancellationToken);
    }

    private HttpClient GetCloudflareApiHttpClient(string authToken)
    {
        // ReSharper disable once InvertIf
        if (_httpClient == null)
        {
            _httpClient = _httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(CloudflareApiAddress);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        }

        return _httpClient;
    }
}