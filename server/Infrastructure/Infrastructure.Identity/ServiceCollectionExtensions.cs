using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Identity;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        var identityConfig = configuration.GetSection("Authentication");

        // If we don't have the "Authentication" node in settings then skip this
        if (!identityConfig.Exists()) return services;

        // This is configured directly from AppSettings
        // - see https://auth0.com/blog/whats-new-in-dotnet-7-for-authentication-and-authorization/
        services.AddAuthentication().AddJwtBearer();

        return services;
    }
}
