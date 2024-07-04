using Microsoft.Extensions.DependencyInjection;
using MyDDNS.Core.IP;

namespace MyDDNS.CLI;

// ReSharper disable once InconsistentNaming
public class MyDDNSApp
{
    private readonly ServiceProvider _serviceProvider;

    // TODO: Change settings below to read from JSON
    private readonly TimeSpan _cycleInterval = TimeSpan.FromSeconds(15);

    public MyDDNSApp(ServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await RunCycleAsync(cancellationToken);
            await Task.Delay(_cycleInterval, cancellationToken);
        }
    }

    private async Task RunCycleAsync(CancellationToken cancellationToken)
    {
        var ipAddressProvider = _serviceProvider.GetRequiredService<IIpAddressProvider>();
        var ip = await ipAddressProvider.GetIpAddressAsync(cancellationToken);
        Console.WriteLine("Got IP {0}", ip);
    }
}