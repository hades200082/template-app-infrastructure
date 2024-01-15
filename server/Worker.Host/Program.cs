using Ardalis.GuardClauses;
using Domain.Abstractions;
using Infrastructure.AMQP;
using Infrastructure.Cosmos;
using Infrastructure.Logging;
using Worker.Host;

var host = Host.CreateDefaultBuilder(args)
    .UseLogging()
    .ConfigureServices((context, services) =>
    {
        services.AddCosmos(context.Configuration.GetSection(CosmosOptions.ConfigurationSectionName), context.HostingEnvironment);
        services.AddAmqp(context.HostingEnvironment, Guard.Against.NullOrEmpty(context.Configuration.GetConnectionString("AzureServiceBus")));
        services.AddSingleton<IChangeFeedHandler, ChangeFeedHandler>();
    })
    .Build();

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


