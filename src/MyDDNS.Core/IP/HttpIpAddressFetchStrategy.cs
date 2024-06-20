using System.Net;

namespace MyDDNS.Core.IP;

public class HttpIpAddressFetchStrategy : IIpAddressFetchStrategy
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IEnumerable<Uri> _ipProviders;

    public HttpIpAddressFetchStrategy(IHttpClientFactory httpClientFactory, IEnumerable<Uri> ipProviders)
    {
        _httpClientFactory = httpClientFactory;
        _ipProviders = ipProviders;
    }

    public async Task<IPAddress> GetIpAddressAsync()
    {
        var ipFetchTasks = _ipProviders.Select(async uri =>
        {
            using var httpClient = _httpClientFactory.CreateClient();
            var ipText = await httpClient.GetStringAsync(uri);
            return ipText;
        });

        var firstCompletedIpFetchTask = await Task.WhenAny(ipFetchTasks);
        var ip = await firstCompletedIpFetchTask;

        return IPAddress.Parse(ip);
    }
}