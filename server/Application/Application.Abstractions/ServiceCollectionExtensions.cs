using Microsoft.Extensions.DependencyInjection;

namespace Application.Abstractions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds to dependency injection all concrete classes that could be assigned to type IApplicationService
    /// from all loaded assemblies.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var serviceTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
            x => x.GetTypes().Where(
                s => s is { IsClass: true, IsAbstract: false, IsInterface: false } && s.IsAssignableTo(typeof(IApplicationService))
            )
        );

        foreach (var serviceType in serviceTypes)
        {
            foreach (var interfaceType in serviceType.GetInterfaces())
            {
                services.AddScoped(interfaceType, serviceType);
            }
        }

        return services;
    }
}
