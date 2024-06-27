using System.Net;
using MyDDNS.Core.Dns;
using MyDDNS.Registrar.Cloudflare.Api;
using MyDDNS.Registrar.Cloudflare.Api.Requests;
using MyDDNS.Registrar.Cloudflare.Configuration;

namespace MyDDNS.Registrar.Cloudflare;

public class CloudflareDnsUpdater : IDnsUpdater
{
    private readonly ICloudflareApiAdapter _cloudflareApi;
    private readonly CloudflareDnsConfiguration _configuration;

    public CloudflareDnsUpdater(ICloudflareApiAdapter cloudflareApi, CloudflareDnsConfiguration configuration)
    {
        _cloudflareApi = cloudflareApi ?? throw new ArgumentNullException(nameof(cloudflareApi));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task UpdateDnsAsync(IPAddress ip, CancellationToken cancellationToken = default)
    {
        foreach (var entry in _configuration.Dns)
        {
            var response = await _cloudflareApi.GetDnsRecordsAsync(_configuration.ZoneIdentifier, entry.Name, cancellationToken);

            if (response?.Result == null)
            {
                // TODO: Log the problem
                continue;
            }

            foreach (var record in response.Result)
            {
                var payload = new PatchDnsRecordRequest {
                    // Entry settings from configuration
                    Name = entry.Name,
                    Proxied = entry.Proxied,
                    Ttl = entry.Ttl,
                    // IP to be changed
                    Content = ip.ToString(),
                    // Keep the same type
                    Type = record.Type
                };
                
                await _cloudflareApi.PatchDnsRecordAsync(_configuration.ZoneIdentifier, record.Id!, payload, cancellationToken);
                // TODO: Log if failed to patch
            }
        }
    }
}