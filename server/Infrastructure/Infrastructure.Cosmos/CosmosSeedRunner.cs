using Domain.DataSeedAbstractions;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Cosmos;

public class CosmosSeedRunner : IDataSeedRunner
{
    private readonly IHostEnvironment _environment;
    private readonly IEnumerable<IDataSeed> _dataSeeds;

    public CosmosSeedRunner(IHostEnvironment environment, IEnumerable<IDataSeed> dataSeeds)
    {
        _environment = environment;
        _dataSeeds = dataSeeds;
    }

    // protected CosmosSeedRunner(IHostEnvironment environment, IDataSeed dataSeeds)
    // {
    //     _environment = environment;
    //     _dataSeeds = new []{dataSeeds};
    // }

    public async Task ExecuteSeedsAsync(CancellationToken cancellationToken)
    {
        var seedPriorityGroups = _dataSeeds
            .Where(x => x is ICosmosDataSeed && x.TargetEnvironment == _environment.EnvironmentName)
            .GroupBy(x => x.Priority)
            .OrderBy(x => x.Key);

        foreach (var seedGroup in seedPriorityGroups)
        {
            // Wait for each priority group to finish before starting the next
            await Task.WhenAll(seedGroup.Select(seed => seed.ExecuteAsync(cancellationToken))).ConfigureAwait(false);
        }
    }
}
