namespace MyDDNS.IP.ThirdParty;

public class IcanhazIpAddressFetchStrategy : HttpIpAddressFetchStrategy
{
    public IcanhazIpAddressFetchStrategy(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    protected override Uri GetServiceUri() => new Uri("https://ipv4.icanhazip.com");
}