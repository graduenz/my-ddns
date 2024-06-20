using System.Net;

namespace MyDDNS.Core.IP;

public class DefaultIpAddressProvider : IIpAddressProvider
{
    private readonly IEnumerable<IIpAddressFetchStrategy> _fetchStrategies;

    public DefaultIpAddressProvider(IEnumerable<IIpAddressFetchStrategy> fetchStrategies)
    {
        _fetchStrategies = fetchStrategies;
    }
    
    public async Task<IPAddress> GetIpAddressAsync()
    {
        foreach (var strategy in _fetchStrategies)
        {
            var ip = await strategy.GetIpAddressAsync();

            if (!Equals(ip, IPAddress.None))
                return ip;
        }

        throw new InvalidOperationException("Failed to fetch the IP address: None of the strategies succeeded.");
    }
}