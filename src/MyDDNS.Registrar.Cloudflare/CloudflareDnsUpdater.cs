using MyDDNS.Core.Dns;
using MyDDNS.Registrar.Cloudflare.Configuration;

namespace MyDDNS.Registrar.Cloudflare;

public class CloudflareDnsUpdater : IDnsUpdater
{
    public Task UpdateDnsAsync(IDnsConfiguration configuration)
    {
        if (configuration is not CloudflareDnsConfiguration providerSpecificConfiguration)
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