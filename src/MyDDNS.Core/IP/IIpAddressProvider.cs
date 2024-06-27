using System.Net;

namespace MyDDNS.Core.IP;

public interface IIpAddressProvider
{
    Task<IPAddress> GetIpAddressAsync(CancellationToken cancellationToken = default);
}