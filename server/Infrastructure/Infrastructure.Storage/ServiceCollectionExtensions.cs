﻿using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Shared.Core;

namespace Infrastructure.Storage;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStorage(
        this IServiceCollection services,
        IConfigurationSection configuration,
        IHostEnvironment environment
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        if (!environment.IsLocal())
        {
            services.AddOptions<StorageOptions>()
                .Bind(configuration)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddSingleton<BlobServiceClient>(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<StorageOptions>>().Value;
                return new BlobServiceClient(
                    new Uri($"https://{options.AccountName}.blob.core.windows.net"),
                    new StorageSharedKeyCredential(options.AccountName, options.AccountKey)
                );
            });
        }
        else
        {
            // If we're running locally let's ensure we're connecting to local services
            services.AddSingleton<BlobServiceClient>(_ =>
                new BlobServiceClient("DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;")
            );
        }


        return services;
    }
}
