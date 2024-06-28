using System.Net;
using Microsoft.Extensions.Logging;

namespace MyDDNS.Core.IP;

public class DefaultIpAddressProvider : IIpAddressProvider
{
    private readonly ILogger<DefaultIpAddressProvider> _logger;
    private readonly IEnumerable<IIpAddressFetchStrategy> _fetchStrategies;

    public DefaultIpAddressProvider(
        ILogger<DefaultIpAddressProvider> logger,
        IEnumerable<IIpAddressFetchStrategy> fetchStrategies)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fetchStrategies = fetchStrategies ?? throw new ArgumentNullException(nameof(fetchStrategies));
        
        if (!fetchStrategies.Any())
            throw new ArgumentException("At least one IP fetch strategy must be specified.", nameof(fetchStrategies));
    }
    
    public async Task<IPAddress> GetIpAddressAsync(CancellationToken cancellationToken = default)
    {
        foreach (var strategy in _fetchStrategies)
        {
            var ip = await strategy.GetIpAddressAsync(cancellationToken);

            var strategyName = strategy.GetType().Name;
            _logger.LogTrace("Got IP {Ip} using {Strategy}.", ip, strategyName);

            if (Equals(ip, IPAddress.None))
                continue;
            
            _logger.LogInformation("Using IP {Ip} got from {Strategy}.", ip, strategyName);
            return ip;
        }

        throw new InvalidOperationException("Failed to fetch the IP address: None of the strategies succeeded.");
    }
}