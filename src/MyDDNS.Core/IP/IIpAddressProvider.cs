using System.Net;

namespace MyDDNS.Core.IP;

/// <summary>
/// Defines a provider to get the user's IP address.
/// </summary>
public interface IIpAddressProvider
{
    /// <summary>
    /// Provides the user's IP address.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>The provided user's IP address.</returns>
    /// <exception cref="InvalidOperationException">When the IP address could not be found.</exception>
    Task<IPAddress> GetIpAddressAsync(CancellationToken cancellationToken = default);
}