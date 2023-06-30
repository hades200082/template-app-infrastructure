namespace Domain.DataSeedAbstractions;

public interface IDataSeedRunner
{
    Task ExecuteSeedsAsync(CancellationToken cancellationToken);
}
