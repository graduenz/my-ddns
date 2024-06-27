using System.Net;
using System.Text.RegularExpressions;
using MyDDNS.Core.IP;

namespace MyDDNS.Registrar.Cloudflare;

public partial class CloudflareIpAddressFetchStrategy : IIpAddressFetchStrategy
{
    private static readonly Uri CloudflareTraceUri = new("https://cloudflare.com/cdn-cgi/trace");
    
    [GeneratedRegex(@"ip=(?<ip>(\b25[0-5]|\b2[0-4][0-9]|\b[01]?[0-9][0-9]?)(\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3})")]
    private static partial Regex IpExtractRegex();
    
    private readonly IHttpClientFactory _httpClientFactory;

    public CloudflareIpAddressFetchStrategy(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }
    
    public async Task<IPAddress> GetIpAddressAsync(CancellationToken cancellationToken = default)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        var traceResponseText = await httpClient.GetStringAsync(CloudflareTraceUri, cancellationToken);

        var match = IpExtractRegex().Match(traceResponseText);
        var success = match.Success && match.Groups["ip"].Success;

        return success && IPAddress.TryParse(match.Groups["ip"].Value, out var ip)
            ? ip
            : IPAddress.None;
    }
}