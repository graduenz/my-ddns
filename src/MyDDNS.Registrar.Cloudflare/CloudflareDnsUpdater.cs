using MyDDNS.Cloudflare.Configuration;
using MyDDNS.Core.Dns;

namespace MyDDNS.Cloudflare;

public class CloudflareDnsUpdater : IDnsUpdater
{
    public Task UpdateDnsAsync(IDnsConfiguration configuration)
    {
        var providerSpecificConfiguration = configuration as CloudflareDnsConfiguration;

        if (providerSpecificConfiguration is null)
            throw new ArgumentException(
                $"Configuration for Cloudflare DNS updater must be an instance of {nameof(CloudflareDnsConfiguration)}",
                nameof(configuration));

        return UpdateDnsInternalAsync(providerSpecificConfiguration);
    }

    private async Task UpdateDnsInternalAsync(CloudflareDnsConfiguration configuration)
    {
        throw new NotImplementedException();
    }
}