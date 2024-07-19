using MyDDNS.Core.IP;
using MyDDNS.Registrar.Cloudflare;

namespace MyDDNS.Worker.Configuration;

internal static class IpAddressProvidersConfigLoader
{
    public static void AddIpAddressProvider(IServiceCollection services, IConfigurationSection configSection)
    {
        var ipProviderSections = configSection.GetSection("IpAddressProviders").GetChildren().ToList();

        if (ipProviderSections.Count == 0)
            throw new InvalidOperationException("Missing IpAddressProviders section.");

        foreach (var providerSection in ipProviderSections)
        {
            var name = providerSection["Name"];
            switch (name)
            {
                case "Http":
                    AddHttp(services, providerSection);
                    continue;

                case "Cloudflare":
                    AddCloudflare(services);
                    continue;

                default:
                    throw new InvalidOperationException($"'{name}' is not recognized on IpAddressProviders.");
            }
        }

        services.AddSingleton<IIpAddressProvider, DefaultIpAddressProvider>();
    }
    
    private static void AddHttp(IServiceCollection services, IConfigurationSection providerSection)
    {
        var uriSections = providerSection.GetSection("Uris").GetChildren().ToList();

        if (uriSections.Count == 0)
            throw new InvalidOperationException("Missing Uris section.");

        var ipProviderUris = uriSections
            .Select(m => new Uri(m.Value ?? throw new InvalidOperationException("Missing URI value.")))
            .ToList();

        services
            .AddSingleton<HttpIpAddressProviderList>(_ => new HttpIpAddressProviderList(ipProviderUris))
            .AddSingleton<IIpAddressFetchStrategy, HttpIpAddressFetchStrategy>();
    }

    private static void AddCloudflare(IServiceCollection services)
    {
        services
            .AddSingleton<IIpAddressFetchStrategy, CloudflareIpAddressFetchStrategy>();
    }
}