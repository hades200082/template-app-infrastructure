using Domain.Abstractions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Core;

namespace Infrastructure.Cosmos;

public class ChangeFeedProvider : IChangeFeedProvider
{
    private readonly ICosmosProvider _cosmosProvider;
    private readonly IConfiguration _configuration;
    private readonly IChangeFeedHandler _changeFeedHandler;
    private readonly ILogger<ChangeFeedProvider> _logger;

    public ChangeFeedProvider(
        ICosmosProvider cosmosProvider,
        IConfiguration configuration,
        IChangeFeedHandler changeFeedHandler,
        ILogger<ChangeFeedProvider> logger)
    {
        _cosmosProvider = cosmosProvider;
        _configuration = configuration;
        _changeFeedHandler = changeFeedHandler;
        _logger = logger;
    }

    public async Task<ChangeFeedProcessor> StartChangeFeedProcessorAsync<T>(string processorName, CancellationToken cancellationToken = default)
    {
        var leaseContainer = await _cosmosProvider.GetLeaseContainerAsync(cancellationToken).ConfigureAwait(false);
        var container = await _cosmosProvider.GetContainerAsync(cancellationToken).ConfigureAwait(false);

        // Try to get the App Service instance ID first - this allows scaling app services out to multiple instances safely
        var instanceName = _configuration.GetValue<string>("WEBSITE_INSTANCE_ID");

        // If the App Service instance ID is not set, use the current machine name and process ID instead
        // This allows the process to be run as a Windows service with multiple instances on a single VM
        if (string.IsNullOrEmpty(instanceName))
            instanceName = $"{Environment.MachineName}_{Environment.ProcessId}";

        var changeFeedProcessor = container.GetChangeFeedProcessorBuilder<Entity>(processorName, _changeFeedHandler.HandleChangeFeedAsync)
            .WithLeaseContainer(leaseContainer)
            .WithInstanceName(instanceName)
            .WithPollInterval(TimeSpan.FromMinutes(1))
            .Build();

        _logger.LogSimpleTrace("Starting change feed processor");
        await changeFeedProcessor.StartAsync().ConfigureAwait(false);
        _logger.LogSimpleTrace("Change feed processor started");
        return changeFeedProcessor;
    }

}
