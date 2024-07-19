using MyDDNS.Core.Dns;
using MyDDNS.Core.IP;
using MyDDNS.Worker.Configuration;

namespace MyDDNS.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ProcessConfig _processConfig;
    private readonly IIpAddressProvider _ipAddressProvider;
    private readonly IEnumerable<IDnsUpdater> _dnsUpdaters;

    public Worker(ILogger<Worker> logger, ProcessConfig processConfig, IIpAddressProvider ipAddressProvider,
        IEnumerable<IDnsUpdater> dnsUpdaters)
    {
        _logger = logger;
        _processConfig = processConfig;
        _ipAddressProvider = ipAddressProvider;
        _dnsUpdaters = dnsUpdaters;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var ip = await _ipAddressProvider.GetIpAddressAsync(stoppingToken);

            foreach (var updater in _dnsUpdaters)
            {
                await updater.UpdateDnsAsync(ip, stoppingToken);
            }
            
            await Task.Delay(_processConfig.CycleInterval, stoppingToken);
        }
    }
}