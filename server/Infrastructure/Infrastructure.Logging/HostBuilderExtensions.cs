using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sentry;
using Serilog;
using Serilog.Events;
using Shared.Core;

namespace Infrastructure.Logging;

public static class HostBuilderExtensions
{
    public static IHostBuilder UseLogging(
        this IHostBuilder builder
    )
    {
        builder.UseSerilog((context, serviceProvider, cfg) =>
        {
            var sentryDsn = context.Configuration["SentryDsn"];
            var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
            cfg.Enrich.FromLogContext();

            if (environment.IsLocal())
            {
                cfg.WriteTo.Console();
                cfg.WriteTo.Debug();
            }

            if (!string.IsNullOrWhiteSpace(sentryDsn))
            {
                cfg.WriteTo.Sentry(options =>
                {
                    options.Environment = environment.EnvironmentName;
                    options.InitializeSdk = true;
                    options.MinimumEventLevel = environment.IsLocal() ? LogEventLevel.Debug : LogEventLevel.Warning;
                    options.Dsn = context.Configuration["SentryDsn"];
                    options.DeduplicateMode = DeduplicateMode.All;
                    options.IsEnvironmentUser = false;
                    options.SendDefaultPii = true;
                });
            }
        });

        return builder;
    }
}