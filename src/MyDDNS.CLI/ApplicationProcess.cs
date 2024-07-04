using MyDDNS.Core.IP;

namespace MyDDNS.CLI;

public class ApplicationProcess
{
    private readonly IIpAddressProvider _ipAddressProvider;

    // TODO: Change settings below to read from JSON
    private readonly TimeSpan _cycleInterval = TimeSpan.FromSeconds(15);

    public ApplicationProcess(IIpAddressProvider ipAddressProvider)
    {
        _ipAddressProvider = ipAddressProvider ?? throw new ArgumentNullException(nameof(ipAddressProvider));
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
        var ip = await _ipAddressProvider.GetIpAddressAsync(cancellationToken);
        Console.WriteLine("Got IP {0}", ip);
    }
}