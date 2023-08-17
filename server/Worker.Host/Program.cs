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

var changeFeedProcessor = await host.Services
    .GetRequiredService<IChangeFeedProvider>()
    .StartChangeFeedProcessorAsync<Entity>("Worker");

host.Run();

// Anything that needs to be gracefully closed down can go here.
await changeFeedProcessor.StopAsync();
