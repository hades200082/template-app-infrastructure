using Ardalis.GuardClauses;
using Domain.DataSeedAbstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Domain.DataSeeds;

public static class HostExtensions
{
    public static async Task ExecuteDataSeedingAsync(this IHost app, CancellationToken cancellationToken)
    {
        Guard.Against.Null(app);

        var runners = app.Services.GetServices<IDataSeedRunner>();
        await Task.WhenAll(runners.Select(x => x.ExecuteSeedsAsync(cancellationToken)));
    }
}
