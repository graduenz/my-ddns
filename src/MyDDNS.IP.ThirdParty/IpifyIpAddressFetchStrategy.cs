namespace MyDDNS.IP.ThirdParty;

public class IpifyIpAddressFetchStrategy : HttpIpAddressFetchStrategy
{
    public IpifyIpAddressFetchStrategy(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    protected override Uri GetServiceUri() => new Uri("https://api.ipify.org");
}