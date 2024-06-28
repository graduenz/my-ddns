using System.Net;

namespace MyDDNS.Core.IP;

/// <summary>
/// Defines a strategy to fetch the user's IP address.
/// </summary>
public interface IIpAddressFetchStrategy
{
    /// <summary>
    /// Loose strategy implementation to get the IP address.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>The user's IP address or <see cref="IPAddress.None"/> on any failure.</returns>
    Task<IPAddress> GetIpAddressAsync(CancellationToken cancellationToken = default);
}