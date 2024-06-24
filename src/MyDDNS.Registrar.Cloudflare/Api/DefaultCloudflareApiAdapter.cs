using System.Net.Http.Headers;
using System.Net.Http.Json;
using MyDDNS.Registrar.Cloudflare.Api.Models;
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
        _httpClientFactory = httpClientFactory;
        _authEmail = authEmail ?? throw new ArgumentNullException(nameof(authEmail));
        _authToken = authToken ?? throw new ArgumentNullException(nameof(authToken));
    }

    public async Task<List<CloudflareDnsRecord>> GetDnsRecordsAsync(string zoneIdentifier, string recordName)
    {
        var requestUri = $"{zoneIdentifier}/dns_records?type=A&name={recordName}";
        
        using var httpClient = GetCloudflareApiHttpClient();
        var dnsRecords = await httpClient.GetFromJsonAsync<List<CloudflareDnsRecord>>(requestUri);

        return dnsRecords ?? throw new InvalidOperationException("Could not GET any DNS record from Cloudflare");
    }

    public async Task<PatchDnsRecordResponse> PatchDnsRecord(string zoneIdentifier, string recordId,
        PatchDnsRecordRequest payload)
    {
        var requestUri = $"{zoneIdentifier}/dns_records/{recordId}";
        
        using var httpClient = GetCloudflareApiHttpClient();
        var response = await httpClient.PatchAsJsonAsync(requestUri, payload);

        var responseObject = response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<PatchDnsRecordResponse>()
            : null;

        return responseObject ?? throw new InvalidOperationException("Could not PATCH the DNS record on Cloudflare");
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