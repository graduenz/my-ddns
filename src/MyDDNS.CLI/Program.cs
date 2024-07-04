using MyDDNS.CLI;

var serviceProvider = DependencyInjection.ConfigureAppDependencies();
var app = new MyDDNSApp(serviceProvider);
var cts = new CancellationTokenSource();

Console.CancelKeyPress += (sender, eventArgs) =>
{
    Console.WriteLine("Cancel event triggered");
    cts.Cancel();
    eventArgs.Cancel = true;
};

await app.RunAsync(cts.Token);