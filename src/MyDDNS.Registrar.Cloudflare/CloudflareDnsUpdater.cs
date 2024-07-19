using System.Net;
using Microsoft.Extensions.Logging;
using MyDDNS.Core.Dns;
using MyDDNS.Registrar.Cloudflare.Api;
using MyDDNS.Registrar.Cloudflare.Api.Models;
using MyDDNS.Registrar.Cloudflare.Api.Requests;
using MyDDNS.Registrar.Cloudflare.Api.Responses;
using MyDDNS.Registrar.Cloudflare.Configuration;

namespace MyDDNS.Registrar.Cloudflare;

/// <summary>
/// Implementation of <see cref="IDnsUpdater"/> that updates DNS records on Cloudflare.
/// </summary>
public class CloudflareDnsUpdater : IDnsUpdater
{
    private readonly ILogger<CloudflareDnsUpdater> _logger;
    private readonly ICloudflareApiAdapter _cloudflareApi;
    private readonly IEnumerable<CloudflareDomainConfig> _domains;

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
        IEnumerable<CloudflareDomainConfig> domains)
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

            if (CheckResponseErrors(response))
                continue;

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

    private async Task PatchDnsRecordAsync(List<CloudflareDnsRecord> records, CloudflareDomainConfig domain,
        IPAddress ip, CancellationToken cancellationToken)
    {
        foreach (var record in records)
        {
            if (record.Content == ip.ToString())
            {
                _logger.LogInformation("Skipping update: IP {Ip} for {Domain} is already set.",
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

            var response = await _cloudflareApi.PatchDnsRecordAsync(domain.ApiToken, domain.ZoneIdentifier,
                record.Id!, payload, cancellationToken);

            if (CheckResponseErrors(response))
                continue;

            if (response is { Success: true })
            {
                _logger.LogInformation("IP {OldIp} was replaced by {NewIp} for {Domain} via Cloudflare API.",
                    record.Content, ip, record.Name);
            }
            else
            {
                _logger.LogError("Failed to replace IP {OldIp} by {NewIp} for {Domain} via Cloudflare API.",
                    record.Content, ip, record.Name);
            }
        }
    }

    /// <summary>
    /// Verifies and logs possible errors that can come in the response
    /// </summary>
    /// <param name="response">The parsed response from Cloudflare API.</param>
    /// <typeparam name="TResult">The response Result type.</typeparam>
    /// <returns>A boolean telling if there were any errors.</returns>
    private bool CheckResponseErrors<TResult>(CloudflareResponse<TResult>? response)
    {
        if (response is { Success: true })
            return false;

        _logger.LogError("Found one or more errors in Cloudflare API response: {Message}.", BuildErrorsMessage());
        return true;

        string BuildErrorsMessage()
        {
            if (response is { Errors: not null })
                return string.Join(", ", response.Errors.Select(err => $"{err.Code} - {err.Message}"));

            return response == null
                ? "Response was NULL"
                : "Response.Errors was NULL";
        }
    }
}