using System.Net;

namespace MyDDNS.Core.IP;

public class DefaultIpAddressProvider : IIpAddressProvider
{
    private readonly IEnumerable<IIpAddressFetchStrategy> _fetchStrategies;

    public DefaultIpAddressProvider(IEnumerable<IIpAddressFetchStrategy> fetchStrategies)
    {
        _fetchStrategies = fetchStrategies ?? throw new ArgumentNullException(nameof(fetchStrategies));
        
        if (!fetchStrategies.Any())
            throw new ArgumentException("At least one IP fetch strategy must be specified.", nameof(fetchStrategies));
    }
    
    public async Task<IPAddress> GetIpAddressAsync(CancellationToken cancellationToken = default)
    {
        foreach (var strategy in _fetchStrategies)
        {
            var ip = await strategy.GetIpAddressAsync(cancellationToken);

            if (!Equals(ip, IPAddress.None))
                return ip;
        }

        throw new InvalidOperationException("Failed to fetch the IP address: None of the strategies succeeded.");
    }
}