using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyDDNS.Core.IP;
using MyDDNS.Registrar.Cloudflare;

namespace MyDDNS.CLI;

public static class DependencyInjection
{
    public static ServiceProvider ConfigureAppDependencies()
    {
        var services = new ServiceCollection();
        services
            .AddMyDDNS()
            .AddLogging(b => SetupLogging(b))
            .AddHttpClient();

        return services.BuildServiceProvider();
    }

    // TODO: Load from appsettings
    private static ILoggingBuilder SetupLogging(ILoggingBuilder builder)
    {
        return builder
            .AddConsole()
            .SetMinimumLevel(LogLevel.Debug);
    }

    // ReSharper disable once InconsistentNaming
    private static IServiceCollection AddMyDDNS(this IServiceCollection services)
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