using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Cosmos;

public static class HealthChecksBuilderExtensions
{
    public static IHealthChecksBuilder AddCosmos(this IHealthChecksBuilder builder)
    {
        return builder.AddCosmosDb((provider, cfg) =>
        {
            var options = provider.GetRequiredService<IOptions<CosmosOptions>>().Value;
            cfg.DatabaseId = options.DatabaseId;
            cfg.ContainerIds = new[] { options.ContainerName }!;
        });
    }
}
