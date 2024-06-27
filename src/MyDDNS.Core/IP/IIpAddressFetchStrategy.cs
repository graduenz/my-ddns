using System.Net;

namespace MyDDNS.Core.IP;

public interface IIpAddressFetchStrategy
{
    Task<IPAddress> GetIpAddressAsync(CancellationToken cancellationToken = default);
}