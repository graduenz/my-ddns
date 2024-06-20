using System.Net;
using System.Text.RegularExpressions;
using MyDDNS.Core.IP;

namespace MyDDNS.Registrar.Cloudflare;

public partial class CloudflareIpAddressFetchStrategy : IIpAddressFetchStrategy
{
    private static readonly Uri CloudflareTraceUri = new("https://cloudflare.com/cdn-cgi/trace");
    
    [GeneratedRegex(@"ip=(?<ip>.+)")]
    private static partial Regex IpExtractRegex();
    
    private readonly IHttpClientFactory _httpClientFactory;

    protected CloudflareIpAddressFetchStrategy(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<IPAddress> GetIpAddressAsync()
    {
        using var httpClient = _httpClientFactory.CreateClient();
        var traceResponseText = await httpClient.GetStringAsync(CloudflareTraceUri);

        var match = IpExtractRegex().Match(traceResponseText);
        var success = match.Success && match.Groups["ip"].Success;

        return success
            ? IPAddress.Parse(match.Groups["ip"].Value)
            : IPAddress.None;
    }
}