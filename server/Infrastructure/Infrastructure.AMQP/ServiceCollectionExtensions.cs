using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.AMQP;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAmqp(
        this IServiceCollection services,
        IHostEnvironment environment,
        string azureServiceBusConnectionString
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
                    cfg.ConfigureEndpoints(context);
                });
            else
                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(azureServiceBusConnectionString);
                    cfg.ConfigureEndpoints(context);
                });
        });

        return services;
    }
}
