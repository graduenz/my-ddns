using System.Net;
using Microsoft.Extensions.Logging;

namespace MyDDNS.Core.IP;

public class HttpIpAddressFetchStrategy : IIpAddressFetchStrategy
{
    private readonly ILogger<HttpIpAddressFetchStrategy> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IEnumerable<Uri> _ipProviders;

    public HttpIpAddressFetchStrategy(
        ILogger<HttpIpAddressFetchStrategy> logger,
        IHttpClientFactory httpClientFactory,
        IEnumerable<Uri> ipProviders)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _ipProviders = ipProviders ?? throw new ArgumentNullException(nameof(ipProviders));

        if (!ipProviders.Any())
            throw new ArgumentException("At least one IP provider must be specified.", nameof(ipProviders));
    }

    public async Task<IPAddress> GetIpAddressAsync(CancellationToken cancellationToken = default)
    {
        var ipFetchTasks = _ipProviders.Select(async uri =>
        {
            using var httpClient = _httpClientFactory.CreateClient();
            var ipString = await httpClient.GetStringAsync(uri, cancellationToken);
            return (ipString, uri);
        });

        var firstCompletedIpFetchTask = await Task.WhenAny(ipFetchTasks);
        var (actualIpString, actualUri) = await firstCompletedIpFetchTask;
        
        _logger.LogTrace("Got IP {Ip} from {Uri}.", actualIpString, actualUri);

        return IPAddress.TryParse(actualIpString, out var ip) ? ip : IPAddress.None;
    }
}