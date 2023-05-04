using Microsoft.Extensions.Hosting;

namespace Shared.Core;

public static class HostEnvironmentExtensions
{
    public static bool IsLocal(this IHostEnvironment environment)
    {
        return environment.IsEnvironment("Local");
    }
}