namespace MyDDNS.Worker.Configuration;

public static class ConfigurationLoader
{
    public static void LoadAndAddServices(IServiceCollection services, IConfigurationSection configSection)
    {
        AddProcessConfig(services, configSection);
        IpAddressProvidersConfigLoader.AddIpAddressProvider(services, configSection);
        DomainsConfigLoader.AddDnsUpdaters(services, configSection);
    }

    private static void AddProcessConfig(IServiceCollection services, IConfigurationSection configSection)
    {
        var processConfig = new ProcessConfig();
        configSection.GetSection("Process").Bind(processConfig);

        services.AddSingleton<ProcessConfig>(processConfig);
    }
}