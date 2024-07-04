using Microsoft.Extensions.DependencyInjection;
using MyDDNS.CLI;
using MyDDNS.Core.IP;

using var cancellationTokenSource = new CancellationTokenSource();

SetupCancellationEvent();

Console.WriteLine("Setting up the application dependencies.");
var serviceProvider = DependencyInjection.ConfigureAppDependencies();

var app = new ApplicationProcess(
    ipAddressProvider: serviceProvider.GetRequiredService<IIpAddressProvider>()
);

try
{
    Console.WriteLine("Starting MyDDNS application.");
    await app.RunAsync(cancellationTokenSource.Token);
}
catch (OperationCanceledException) when (cancellationTokenSource.IsCancellationRequested)
{
    Console.WriteLine("Shutting down, please wait.");
}

return;

void SetupCancellationEvent()
{
    Console.CancelKeyPress += (sender, eventArgs) =>
    {
        Console.WriteLine("Cancel event triggered.");
        // ReSharper disable once AccessToDisposedClosure
        cancellationTokenSource.Cancel();
        eventArgs.Cancel = true;
    };
}