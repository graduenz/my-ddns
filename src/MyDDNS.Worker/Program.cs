using MyDDNS.Worker;
using MyDDNS.Worker.Configuration;

var builder = Host.CreateApplicationBuilder(args);

ConfigurationLoader.LoadAndAddServices(builder.Services, builder.Configuration.GetSection("MyDDNS"));

builder.Services.AddHostedService<Worker>();

builder.Services.AddHttpClient();

var host = builder.Build();
host.Run();