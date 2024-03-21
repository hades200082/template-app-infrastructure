using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.AMQP;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAmqp(
        this IServiceCollection services,
        IHostEnvironment environment,
        string? azureServiceBusConnectionString
    )
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumers(AppDomain.CurrentDomain.GetAssemblies());

            if(environment.IsEnvironment("Local"))
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter("queue-",false));

                    // Ensure Exchanges are named without reference to C# namespaces
                    cfg.MessageTopology.SetEntityNameFormatter(new CustomerEntityNameFormatter());
                });
            else
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(azureServiceBusConnectionString);

                // Configure MT to use Azure's built-in dead letter features
                // instead of using extra queues
                // https://masstransit.io/documentation/configuration/transports/azure-service-bus#using-service-bus-dead-letter-queues
                x.AddConfigureEndpointsCallback((_, cfg) =>
                {
                    if (cfg is IServiceBusReceiveEndpointConfigurator sb)
                    {
                        sb.ConfigureDeadLetterQueueDeadLetterTransport();
                        sb.ConfigureDeadLetterQueueErrorTransport();
                    }
                });
                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(azureServiceBusConnectionString);
                    cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter("queue-", false));

                    // Ensure Topics are named without reference to C# namespaces
                    cfg.MessageTopology.SetEntityNameFormatter(new CustomerEntityNameFormatter());
                });
            }
        });

        return services;
    }
}
