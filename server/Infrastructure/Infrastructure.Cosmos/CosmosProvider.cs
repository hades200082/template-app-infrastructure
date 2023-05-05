using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Shared.Core;

namespace Infrastructure.Cosmos;

internal sealed class CosmosProvider : ICosmosProvider
{
    private readonly CosmosOptions _options;
    private readonly CosmosClient _client;
    private static Container? _container;
    private static Container? _leaseContainer;

    public CosmosProvider(
        IOptions<CosmosOptions> options,
        IHttpClientFactory httpClientFactory,
        IHostEnvironment environment
    )
    {
        if (!options.Value.Validate())
            throw new EnvironmentConfigurationException("CosmosOptions are not valid. Check that you have created the appropriate environment variables.");

        _options = options.Value;

        var builder = new CosmosClientBuilder(_options.AccountEndpoint, _options.AccountKey)
            .WithSerializerOptions(new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            })
            .WithHttpClientFactory(() => httpClientFactory.CreateClient(nameof(CosmosClient)));

        if (environment.IsLocal())
            builder.WithConnectionModeGateway();

        _client = builder.Build();
    }

    public async Task<Container> GetContainerAsync(CancellationToken cancellationToken = default)
    {
        if (_container is null)
        {
            var db = (await _client.CreateDatabaseIfNotExistsAsync(_options.DatabaseId, cancellationToken: cancellationToken).ConfigureAwait(false)).Database;
            _container = (await db.CreateContainerIfNotExistsAsync(_options.ContainerName, "/_pk", cancellationToken: cancellationToken).ConfigureAwait(false)).Container;
        }

        return _container;
    }

    public async Task<Container> GetLeaseContainerAsync(CancellationToken cancellationToken)
    {
        if (_leaseContainer is null)
        {
            var db = (await _client.CreateDatabaseIfNotExistsAsync(_options.DatabaseId, cancellationToken: cancellationToken).ConfigureAwait(false)).Database;
            _leaseContainer = (await db.CreateContainerIfNotExistsAsync("leases", "/PartitionId", cancellationToken: cancellationToken).ConfigureAwait(false)).Container;
        }

        return _leaseContainer;
    }
}
