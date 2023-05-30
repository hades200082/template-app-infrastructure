using Infrastructure.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Cosmos;

public static class HealthChecksBuilderExtensions
{
    public static IHealthChecksBuilder AddStorage(this IHealthChecksBuilder builder)
    {
        return builder.AddAzureBlobStorage((provider, cfg) =>
        {
            var options = provider.GetRequiredService<IOptions<StorageOptions>>().Value;
            cfg.ContainerName = options.ContainerName;
        });
    }
}
