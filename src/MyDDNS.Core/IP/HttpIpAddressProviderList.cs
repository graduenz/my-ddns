namespace MyDDNS.Core.IP;

public class HttpIpAddressProviderList : List<Uri>
{
    public HttpIpAddressProviderList(IEnumerable<Uri> uris) : base(uris)
    {
    }
}