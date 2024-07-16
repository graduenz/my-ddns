using MyDDNS.Core.IP;
using MyDDNS.Registrar.Cloudflare;

namespace MyDDNS.Worker;

public static class DependencyInjection
{
    public static void AddApplicationDependencies(this IServiceCollection services) => services
        .AddLibraries()
        .AddHttpClient();

    private static IServiceCollection AddLibraries(this IServiceCollection services)
    {
        // TODO: Load the URIs from JSON settings
        var ipProviderUris = new[] { "https://api.ipify.org", "https://ipv4.icanhazip.com" }
            .Select(m => new Uri(m))
            .ToList();

        return services
            .AddScoped<IIpAddressFetchStrategy, CloudflareIpAddressFetchStrategy>()
            .AddScoped<HttpIpAddressProviderList>(_ => new HttpIpAddressProviderList(ipProviderUris))
            .AddScoped<IIpAddressFetchStrategy, HttpIpAddressFetchStrategy>()
            .AddScoped<IIpAddressProvider, DefaultIpAddressProvider>();
    }
}