using System.Net;
using MyDDNS.Core.Dns;
using MyDDNS.Registrar.Cloudflare.Api;
using MyDDNS.Registrar.Cloudflare.Api.Requests;
using MyDDNS.Registrar.Cloudflare.Configuration;

namespace MyDDNS.Registrar.Cloudflare;

public class CloudflareDnsUpdater : IDnsUpdater
{
    private readonly ICloudflareApiAdapter _cloudflareApi;
    private readonly IEnumerable<CloudflareDomainConfiguration> _domains;

    public CloudflareDnsUpdater(ICloudflareApiAdapter cloudflareApi, IEnumerable<CloudflareDomainConfiguration> domains)
    {
        _cloudflareApi = cloudflareApi ?? throw new ArgumentNullException(nameof(cloudflareApi));
        _domains = domains ?? throw new ArgumentNullException(nameof(domains));
        
        if (!domains.Any())
            throw new ArgumentException("At least one domain must be specified.", nameof(domains));
    }

    public async Task UpdateDnsAsync(IPAddress ip, CancellationToken cancellationToken = default)
    {
        foreach (var domain in _domains)
        {
            var response = await _cloudflareApi.GetDnsRecordsAsync(domain.ApiToken, domain.ZoneIdentifier,
                domain.RecordName, cancellationToken);

            if (response?.Result == null)
            {
                // TODO: Log the problem
                continue;
            }

            foreach (var record in response.Result)
            {
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

                await _cloudflareApi.PatchDnsRecordAsync(domain.ApiToken, domain.ZoneIdentifier, record.Id!, payload,
                    cancellationToken);
                // TODO: Log if failed to patch
            }
        }
    }
}