using System.Net;

namespace MyDDNS.Core.IP;

public class HttpIpAddressFetchStrategy : IIpAddressFetchStrategy
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IEnumerable<Uri> _ipProviders;

    public HttpIpAddressFetchStrategy(IHttpClientFactory httpClientFactory, IEnumerable<Uri> ipProviders)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _ipProviders = ipProviders ?? throw new ArgumentNullException(nameof(ipProviders));

        if (!ipProviders.Any())
            throw new ArgumentException("At least one IP provider must be specified.", nameof(ipProviders));
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
        var ipString = await firstCompletedIpFetchTask;

        return IPAddress.TryParse(ipString, out var ip)
            ? IPAddress.None
            : ip ?? IPAddress.None;
    }
}