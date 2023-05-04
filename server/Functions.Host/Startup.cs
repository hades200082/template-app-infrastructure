using Ardalis.GuardClauses;
using Functions.Host;
using Infrastructure.AMQP;
using Infrastructure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly:FunctionsStartup(typeof(Startup))]

namespace Functions.Host;

public sealed class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        Guard.Against.Null(builder);

        // This feels hacky but is apparently the only way to get these things in Azure Functions :(
        var serviceProvider = builder.Services.BuildServiceProvider();
        var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
        var configuration = builder.GetContext().Configuration;

        // Add our own DI
        builder.Services.AddCosmos(configuration, environment);
        builder.Services.AddAmqp(configuration, environment);
    }

}
