using MyDDNS.Core.Dns;
using MyDDNS.Registrar.Cloudflare;
using MyDDNS.Registrar.Cloudflare.Api;
using MyDDNS.Registrar.Cloudflare.Configuration;

namespace MyDDNS.Worker.Configuration;

internal static class DomainsConfigLoader
{
    private const string Cloudflare = "Cloudflare";

    private static readonly string[] SupportedRegistrars = [Cloudflare];

    public static void AddDnsUpdaters(IServiceCollection services, IConfigurationSection configSection)
    {
        var domainSections = configSection.GetSection("Domains").GetChildren().ToList();

        if (domainSections.Count == 0)
            throw new InvalidOperationException("Missing Domains section.");

        ThrowIfUnsupportedRegistrars(domainSections);

        var cloudflareDomainSections = domainSections.Where(m => m["Registrar"] == Cloudflare).ToList();
        if (cloudflareDomainSections.Count > 0)
        {
            AddCloudflare(services, cloudflareDomainSections);
        }
    }

    private static void ThrowIfUnsupportedRegistrars(List<IConfigurationSection> domainSections)
    {
        var unsupportedRegistrars = domainSections
            .Select(m => m["Registrar"])
            .Distinct()
            .Where(m => !SupportedRegistrars.Contains(m))
            .ToList();

        if (unsupportedRegistrars.Count > 0)
            throw new InvalidOperationException(
                $"Unsupported domain registrars: {string.Join(", ", unsupportedRegistrars)}.");
    }

    private static void AddCloudflare(IServiceCollection services, List<IConfigurationSection> cloudflareDomainSections)
    {
        foreach (var domainSection in cloudflareDomainSections)
        {
            services.AddSingleton(ReadCloudflareDomainConfig(domainSection));
        }

        services.AddSingleton<ICloudflareApiAdapter, DefaultCloudflareApiAdapter>();
        services.AddSingleton<IDnsUpdater, CloudflareDnsUpdater>();
    }

    private static CloudflareDomainConfig ReadCloudflareDomainConfig(IConfigurationSection domainSection)
    {
        return new CloudflareDomainConfig(
            domainSection["ApiToken"] ?? throw new InvalidOperationException("Missing ApiToken value."),
            domainSection["ZoneIdentifier"] ?? throw new InvalidOperationException("Missing ZoneIdentifier value."),
            domainSection["RecordName"] ?? throw new InvalidOperationException("Missing RecordName value."),
            bool.Parse(domainSection["Proxied"] ?? throw new InvalidOperationException("Missing Proxied value.")),
            int.Parse(domainSection["Ttl"] ?? throw new InvalidOperationException("Missing Ttl value."))
        );
    }
}