using NioxVF.Agent;
using NioxVF.Agent.Services;
using NioxVF.Domain.Interfaces;
using NioxVF.Domain.Services;

var builder = Host.CreateApplicationBuilder(args);

// Configure services
builder.Services.AddHostedService<Worker>();

// Register domain services
builder.Services.AddSingleton<IHashChain, HashChainService>();

// Register agent services
builder.Services.AddSingleton<IInboxReader, JsonInboxReader>();
builder.Services.AddSingleton<IQrGenerator, PlaceholderQrGenerator>();

// Register HTTP client for API communication
builder.Services.AddHttpClient<IApiClient, ApiClient>();

// Add logging
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

var host = builder.Build();

// Log startup
var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("NioxVF Agent starting...");

host.Run();
