using Domain.DataSeedAbstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.DataSeeds;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataSeeds(this IServiceCollection services)
    {
        var seedRunners =
            // Get all currently loaded assemblies
            AppDomain.CurrentDomain
                .GetAssemblies()
                // Where the assembly contains classes that implement IDataSeed
                .Where(x =>
                    x.DefinedTypes.Any(x => !x.IsAbstract &&
                                            x.IsClass &&
                                            x.IsAssignableTo(typeof(IDataSeedRunner))))
                // and select those IDataSeed classes
                .SelectMany(x => x.DefinedTypes.Where(x=> !x.IsAbstract &&
                                                          x.IsClass &&
                                                          x.IsAssignableTo(typeof(IDataSeedRunner))))
                .ToArray();

        // Loop through and add each one to the dependecy container
        foreach (var seedRunner in seedRunners)
        {
            services.AddTransient(typeof(IDataSeedRunner), seedRunner.AsType());
        }

        var seeds =
            // Get all currently loaded assemblies
            AppDomain.CurrentDomain
            .GetAssemblies()
            // Where the assembly contains classes that implement IDataSeed
            .Where(x =>
                x.DefinedTypes.Any(x => !x.IsAbstract &&
                                        x.IsClass &&
                                        x.IsAssignableTo(typeof(IDataSeed))))
            // and select those IDataSeed classes
            .SelectMany(x => x.DefinedTypes.Where(x=> !x.IsAbstract &&
                                        x.IsClass &&
                                        x.IsAssignableTo(typeof(IDataSeed))))
            .ToArray();

        // Loop through and add each one to the dependecy container
        foreach (var seed in seeds)
        {
            services.AddTransient(typeof(IDataSeed), seed.AsType());
        }

        return services;
    }
}
