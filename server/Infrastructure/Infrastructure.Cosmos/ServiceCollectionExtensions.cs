using System.Text.RegularExpressions;
using Ardalis.GuardClauses;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Shared.Core;

namespace Infrastructure.Cosmos;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCosmos(
        this IServiceCollection services,
        IConfigurationSection configuration,
        IHostEnvironment environment
        )
    {
        if (!environment.IsLocal())
            services.AddOptions<CosmosOptions>()
                .Bind(configuration)
                .ValidateDataAnnotations()
                .ValidateOnStart();

        else // If we're running locally let's ensure we're connecting to local services
        {
            // Protecting against DOS attack by limiting time Regex can run
            var regex = new Regex("[^a-zA-Z0-9]+", RegexOptions.None, TimeSpan.FromSeconds(1));

            var projectName = regex.Replace(Guard.Against.Null(configuration)["ProjectName"] ?? "", "");
            if (string.IsNullOrWhiteSpace(projectName))
                throw new EnvironmentConfigurationException("'ProjectName' must be set in host configuration.");

            services.AddOptions<CosmosOptions>()
                .Configure(c =>
                {
                    c.AccountEndpoint = "https://localhost:8081";
                    c.AccountKey =
                    "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
                    c.DatabaseId = $"{projectName}Database";
                    c.ContainerName = $"{projectName}Container";
                })
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }

        services.AddHttpClient<CosmosClient>()
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();

                if (environment.IsLocal())
                {
#pragma warning disable MA0039
                    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
#pragma warning restore MA0039
                }

                return handler;
            });

        services.AddSingleton<CosmosClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<CosmosOptions>>().Value;
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

            var builder = new CosmosClientBuilder(options.AccountEndpoint, options.AccountKey)
                .WithSerializerOptions(new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                })
                .WithHttpClientFactory(() => httpClientFactory.CreateClient(nameof(CosmosClient)));

            if (environment.IsLocal())
                builder.WithConnectionModeGateway();

            return builder.Build();
        });

        services.AddSingleton<ICosmosProvider, CosmosProvider>();

        services.AddSingleton(typeof(IRepository<>), typeof(CosmosRepository<>));

        return services;
    }

    public static IServiceCollection AddChangeFeedProcessor(this IServiceCollection services)
    {
        services.AddSingleton<IChangeFeedProvider, ChangeFeedProvider>();
        return services;
    }
}
