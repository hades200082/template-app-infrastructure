namespace Domain.DataSeedAbstractions;

public interface IDataSeed
{
    /// <summary>
    /// Lower numbered priority seeds will run before higher numbered priority seeds
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// The seed will only run in the environment specified
    /// </summary>
    string TargetEnvironment { get; }

    Task ExecuteAsync(CancellationToken cancellationToken);
}
