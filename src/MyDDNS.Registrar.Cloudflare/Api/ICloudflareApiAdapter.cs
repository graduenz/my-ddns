using MyDDNS.Registrar.Cloudflare.Api.Requests;
using MyDDNS.Registrar.Cloudflare.Api.Responses;

namespace MyDDNS.Registrar.Cloudflare.Api;

/// <summary>
/// Defines an adapter to the Cloudflare API.
/// </summary>
public interface ICloudflareApiAdapter
{
    /// <summary>
    /// Gets the DNS records.
    /// </summary>
    /// <param name="apiToken">The API token.</param>
    /// <param name="zoneIdentifier">The zone identifier.</param>
    /// <param name="recordName">The record name.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>The parsed response body from the Cloudflare API.</returns>
    Task<GetDnsRecordsResponse?> GetDnsRecordsAsync(string apiToken, string zoneIdentifier, string recordName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Patch a DNS record.
    /// </summary>
    /// <param name="apiToken">The API token.</param>
    /// <param name="zoneIdentifier">The zone identifier.</param>
    /// <param name="recordId">The record ID.</param>
    /// <param name="payload">The request payload.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>The parsed response body from the Cloudflare API.</returns>
    Task<PatchDnsRecordResponse?> PatchDnsRecordAsync(string apiToken, string zoneIdentifier, string recordId,
        PatchDnsRecordRequest payload, CancellationToken cancellationToken = default);
}