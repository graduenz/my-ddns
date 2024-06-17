using System.Net;
using MyDDNS.Core.IP;

namespace MyDDNS.IP.ThirdParty;

public abstract class HttpIpAddressFetchStrategy(IHttpClientFactory httpClientFactory) : IIpAddressFetchStrategy
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    
    protected abstract Uri GetServiceUri();

    public async Task<IPAddress> GetIpAddressAsync()
    {
        using var httpClient = _httpClientFactory.CreateClient();
        var ipText = await httpClient.GetStringAsync(GetServiceUri());

        return IPAddress.Parse(ipText);
    }
}