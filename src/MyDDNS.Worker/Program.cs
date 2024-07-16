using MyDDNS.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddApplicationDependencies();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();