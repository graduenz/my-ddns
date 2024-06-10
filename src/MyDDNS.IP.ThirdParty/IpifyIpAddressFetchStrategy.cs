using System.Net;
using MyDDNS.Core.IP;

namespace MyDDNS.IP.ThirdParty;

public class IpifyIpAddressFetchStrategy : IIpAddressFetchStrategy
{
    public Task<IPAddress> GetIpAddressAsync()
    {
        throw new NotImplementedException();
    }
}