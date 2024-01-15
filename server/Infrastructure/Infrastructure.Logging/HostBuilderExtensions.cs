using Microsoft.Extensions.Configuration;
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
            var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
            cfg.Enrich.FromLogContext();

            if (environment.IsLocal())
            {
                cfg.MinimumLevel.Verbose();
#pragma warning disable CA1305
                cfg.WriteTo.Console();
                cfg.WriteTo.Debug();
#pragma warning restore CA1305
            }
            else
            {
                // Set minimum level _Serilog_ will process - this is different to Sentry's minimum log level below
                // and this value should be more verbose than Sentry's. This ensures that Sentry can track
                // more verbose log events for breadcrumbs when it sends a tracked event to Sentry.io.
                cfg.MinimumLevel.Information();

                // Only use Sentry if we're not running locally and Sentry is configured in appSettings/environment variables
                var sentryConfig = context.Configuration.GetSection("Sentry");
                if (!sentryConfig.Exists()) return;

                var dsn = sentryConfig.GetValue<string>("Dsn");
                var minLevel = sentryConfig.GetValue<LogEventLevel?>("MinimumLogLevel");
                var minBreadcrumb = sentryConfig.GetValue<LogEventLevel?>("MinimumBreadcrumbLevel");

                if (minBreadcrumb is not null && minBreadcrumb >= minLevel)
                    minBreadcrumb = null;

                cfg.WriteTo.Sentry(options =>
                {
                    options.Environment = environment.EnvironmentName;
                    options.InitializeSdk = true;
                    options.MinimumEventLevel = minLevel ?? LogEventLevel.Warning;
                    options.Dsn = dsn;
                    options.DeduplicateMode = DeduplicateMode.All;
                    options.IsEnvironmentUser = false;
                    options.SendDefaultPii = true;
                    options.MinimumBreadcrumbLevel = minBreadcrumb ?? LogEventLevel.Information;

                    var verFilePath = Path.Join(context.HostingEnvironment.ContentRootPath, "ver.txt");
                    if (File.Exists(verFilePath))
                    {
                        var ver = File.ReadAllText(verFilePath);
                        options.Release = ver.Trim();
                    }

                    options.EnableTracing = sentryConfig.GetValue<bool?>("EnableTracing");
                    if (options.EnableTracing ?? false)
                    {
                        options.TracesSampleRate = sentryConfig.GetValue<double?>("TracesSampleRate") ?? 1;
                    }

                    // Try to get the App Service instance ID first - this allows scaling app services out to multiple instances safely
                    var instanceName = context.Configuration.GetValue<string>("WEBSITE_INSTANCE_ID");

                    // If the App Service instance ID is not set, use the current machine name and process ID instead
                    // This allows the process to be run as a Windows service with multiple instances on a single VM
                    // and lets us identify which instance logged an event
                    if (string.IsNullOrEmpty(instanceName))
                        instanceName = $"{Environment.MachineName}_{Environment.ProcessId}";

                    options.ServerName = instanceName;
                });
            }
        });



        return builder;
    }
}
