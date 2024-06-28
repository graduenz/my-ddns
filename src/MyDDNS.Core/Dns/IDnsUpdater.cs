using System.Net;

namespace MyDDNS.Core.Dns;

/// <summary>
/// Defines an updater to update DNS records.
/// </summary>
public interface IDnsUpdater
{
    /// <summary>
    /// Updates the DNS records.
    /// </summary>
    /// <param name="ip">The IP to set on A records.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task UpdateDnsAsync(IPAddress ip, CancellationToken cancellationToken = default);
}