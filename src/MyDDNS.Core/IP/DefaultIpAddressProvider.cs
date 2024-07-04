using System.Net;
using Microsoft.Extensions.Logging;

namespace MyDDNS.Core.IP;

/// <summary>
/// Implementation of <see cref="IIpAddressProvider"/> that returns the IP from the first successful strategy.
/// </summary>
public class DefaultIpAddressProvider : IIpAddressProvider
{
    private readonly ILogger<DefaultIpAddressProvider> _logger;
    private readonly IEnumerable<IIpAddressFetchStrategy> _fetchStrategies;

    /// <summary>
    /// Creates the <see cref="DefaultIpAddressProvider"/>.
    /// </summary>
    /// <param name="logger">An instance of <see cref="ILogger"/>.</param>
    /// <param name="fetchStrategies">One or more strategies to fetch the user's IP address.</param>
    /// <exception cref="ArgumentNullException">When any of the parameters are null.</exception>
    /// <exception cref="ArgumentException">When <paramref name="fetchStrategies"/> parameter is empty.</exception>
    public DefaultIpAddressProvider(
        ILogger<DefaultIpAddressProvider> logger,
        IEnumerable<IIpAddressFetchStrategy> fetchStrategies)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fetchStrategies = fetchStrategies ?? throw new ArgumentNullException(nameof(fetchStrategies));
        
        if (!fetchStrategies.Any())
            throw new ArgumentException("At least one IP fetch strategy must be specified.", nameof(fetchStrategies));
    }
    
    /// <inheritdoc />
    public async Task<IPAddress> GetIpAddressAsync(CancellationToken cancellationToken = default)
    {
        foreach (var strategy in _fetchStrategies)
        {
            var ip = await strategy.GetIpAddressAsync(cancellationToken);

            var strategyName = strategy.GetType().Name;
            _logger.LogDebug("Got IP {Ip} using {Strategy}.", ip, strategyName);

            if (Equals(ip, IPAddress.None))
                continue;
            
            _logger.LogInformation("Using IP {Ip} got from {Strategy}.", ip, strategyName);
            return ip;
        }

        throw new InvalidOperationException("Failed to fetch the IP address: None of the strategies succeeded.");
    }
}