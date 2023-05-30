using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Shared.Core;

namespace Infrastructure.AMQP;

public static class HealthChecksBuilderExtensions
{
    public static IHealthChecksBuilder AddAMQP(this IHealthChecksBuilder builder, IHostEnvironment environment)
    {
        if (environment.IsLocal())
        {
            builder.AddRabbitMQ(rabbitConnectionString: "amqp://guest:guest@localhost/");
        }
        else
        {
            // TODO: Find a way to add Azure Service Bus checks here
            // Or a way to create custom checks with MassTransit
        }

        return builder;
    }
}
