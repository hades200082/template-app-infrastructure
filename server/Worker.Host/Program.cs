using Ardalis.GuardClauses;
using Domain.Abstractions;
using Infrastructure.AMQP;
using Infrastructure.Cosmos;
using Infrastructure.Logging;
using Microsoft.AspNetCore.Builder;
using Worker.Host;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseLogging();

builder.Services.AddCosmos(builder.Configuration.GetSection(CosmosOptions.ConfigurationSectionName), builder.Environment);
builder.Services.AddAmqp(builder.Environment,
    Guard.Against.NullOrEmpty(builder.Configuration.GetConnectionString("AzureServiceBus")));
builder.Services.AddSingleton<IChangeFeedHandler, ChangeFeedHandler>();

var host = builder.Build();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

var changeFeedProcessor = await host.Services
    .GetRequiredService<IChangeFeedProvider>()
    .StartChangeFeedProcessorAsync<Entity>("Worker");

host.Run();

#pragma warning disable CA1031
try
{
    await host.RunAsync().ConfigureAwait(false);
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Application threw an unhandled exception and shut down");
}
finally
{
    // Anything that needs to be gracefully closed down can go here.
    await changeFeedProcessor.StopAsync();
}
#pragma warning restore CA1031
