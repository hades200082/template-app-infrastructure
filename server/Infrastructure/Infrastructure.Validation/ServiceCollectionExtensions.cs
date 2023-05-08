using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Validation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        return services;
    }
}
