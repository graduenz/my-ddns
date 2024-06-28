using System.Net;
using Microsoft.Extensions.Logging;
using MyDDNS.Core.Dns;
using MyDDNS.Registrar.Cloudflare.Api;
using MyDDNS.Registrar.Cloudflare.Api.Models;
using MyDDNS.Registrar.Cloudflare.Api.Requests;
using MyDDNS.Registrar.Cloudflare.Configuration;

namespace MyDDNS.Registrar.Cloudflare;

/// <summary>
/// Implementation of <see cref="IDnsUpdater"/> that updates DNS records on Cloudflare.
/// </summary>
public class CloudflareDnsUpdater : IDnsUpdater
{
    private readonly ILogger<CloudflareDnsUpdater> _logger;
    private readonly ICloudflareApiAdapter _cloudflareApi;
    private readonly IEnumerable<CloudflareDomainConfiguration> _domains;

    /// <summary>
    /// Creates the <see cref="CloudflareDnsUpdater"/>.
    /// </summary>
    /// <param name="logger">An instance of <see cref="ILogger"/>.</param>
    /// <param name="cloudflareApi">An instance of <see cref="ICloudflareApiAdapter"/>.</param>
    /// <param name="domains">One or more domain configurations to update.</param>
    /// <exception cref="ArgumentNullException">When any of the parameters are null.</exception>
    /// <exception cref="ArgumentException">When <paramref name="domains"/> parameter is empty.</exception>
    public CloudflareDnsUpdater(
        ILogger<CloudflareDnsUpdater> logger,
        ICloudflareApiAdapter cloudflareApi,
        IEnumerable<CloudflareDomainConfiguration> domains)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cloudflareApi = cloudflareApi ?? throw new ArgumentNullException(nameof(cloudflareApi));
        _domains = domains ?? throw new ArgumentNullException(nameof(domains));

        if (!domains.Any())
            throw new ArgumentException("At least one domain must be specified.", nameof(domains));
    }
    
    /// <inheritdoc />
    public async Task UpdateDnsAsync(IPAddress ip, CancellationToken cancellationToken = default)
    {
        foreach (var domain in _domains)
        {
            var response = await _cloudflareApi.GetDnsRecordsAsync(domain.ApiToken, domain.ZoneIdentifier,
                domain.RecordName, cancellationToken);

            if (response?.Result == null)
            {
                _logger.LogWarning(
                    "Got NULL response when getting DNS records from Cloudflare API. Zone: {Zone}, domain: {Domain}.",
                    domain.ZoneIdentifier, domain.RecordName);
                continue;
            }

            if (response.Result.Count == 0)
            {
                _logger.LogWarning("Got NO DNS RECORDS from Cloudflare API. Zone: {Zone}, domain: {Domain}.",
                    domain.ZoneIdentifier, domain.RecordName);
                continue;
            }

            await PatchDnsRecordAsync(response.Result, domain, ip, cancellationToken);
        }
    }

    private async Task PatchDnsRecordAsync(List<CloudflareDnsRecord> records, CloudflareDomainConfiguration domain,
        IPAddress ip, CancellationToken cancellationToken)
    {
        foreach (var record in records)
        {
            if (record.Content == ip.ToString())
            {
                _logger.LogInformation("Skipping update: IP {Ip} for {Domain} has not changed.",
                    ip, record.Name);
                continue;
            }

            var payload = new PatchDnsRecordRequest
            {
                // Entry settings from configuration
                Name = domain.RecordName,
                Proxied = domain.Proxied,
                Ttl = domain.Ttl,
                // IP to be changed
                Content = ip.ToString(),
                // Keep the same type (A)
                Type = record.Type
            };

            var patchResponse = await _cloudflareApi.PatchDnsRecordAsync(domain.ApiToken, domain.ZoneIdentifier,
                record.Id!, payload, cancellationToken);

            if (patchResponse is not { Success: true })
            {
                _logger.LogError("Failed to replace IP {OldIp} by {NewIp} for {Domain} via Cloudflare API.",
                    record.Content, ip, record.Name);
            }
        }
    }
}