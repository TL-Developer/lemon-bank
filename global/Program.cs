using global;

var builder = Host.CreateApplicationBuilder(args);

// Add Kafka producer configuration
builder.Services.AddSingleton<Producer>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
