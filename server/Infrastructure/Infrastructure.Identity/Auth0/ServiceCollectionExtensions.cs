using Auth0.AuthenticationApi;
using Auth0.ManagementApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace Infrastructure.Identity.Auth0;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuth0(this IServiceCollection services, IConfigurationSection configuration)
    {
        var auth0Options = services.AddAuth0Options(configuration);

        // Add the authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = $"https://{auth0Options.Domain}";

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = auth0Options.NameClaimType,
                    ValidIssuer = $"https://{auth0Options.Domain}",
                    ValidAudiences = auth0Options.ValidAudiences
                };
            });

        return services;
    }

    public static IServiceCollection AddAuth0AuthenticationApiClient(this IServiceCollection services, IConfigurationSection configuration)
    {
        // Ensure options are registered just in case we're calling this in a worker.
        services.AddAuth0Options(configuration.GetSection(nameof(Auth0Options)));
        services.AddScoped<IAuthenticationApiClient, AuthenticationApiClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<Auth0Options>>();
            return new AuthenticationApiClient(options.Value.Domain);
        });
        return services;
    }

    public static IServiceCollection AddAuth0ManagementApiClient(this IServiceCollection services, IConfigurationSection configuration)
    {
        // If we don't have the "Authentication" node in settings then skip this
        if (!configuration.Exists()) return services;

        // Otherwise bind the options and validate
        services.AddOptions<Auth0ManagementApiOptions>()
            .Bind(configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Setup our HTTP Client
        services.AddHttpClient<ManagementApiClientFactory>(nameof(ManagementApiClientFactory))
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddTransientHttpErrorPolicy(builder =>
                builder.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5)));

        services.AddTransient<IManagementApiClient, ManagementApiClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<Auth0ManagementApiOptions>>();
            var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>()
                .CreateClient(nameof(ManagementApiClientFactory));
            return new ManagementApiClientFactory(httpClient, options).GetClientAsync().Result;
        });

        return services;
    }

    /// <summary>
    /// Gets the base Auth0Options and, if not already registered, registers them with DI
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns>The Auth0Options</returns>
    private static Auth0Options? AddAuth0Options(this IServiceCollection services, IConfigurationSection configuration)
    {
        // Register IOptions<Auth0Options> only if not already registered
        if (services.All(x => x.ServiceType != typeof(IConfigureOptions<Auth0Options>)))
        {
            services.AddOptions<Auth0Options>()
                .Bind(configuration)
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }

        return services.BuildServiceProvider().GetService<IOptions<Auth0Options>>()?.Value;
    }
}
