using Infrastructure.Cosmos;

namespace Domain.DataSeeds.ExampleEntity;

public sealed class UatExampleEntitySeed : ICosmosDataSeed
{
    private readonly IRepository<Entities.ExampleEntity> _repository;
    public int Priority => 0;
    public string TargetEnvironment => "UAT";
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Delete anything that already exists
        var result = await _repository.QueryAsync(cancellationToken);
        var items = result.Data.ToList();
        if (result.HasMore)
        {
            result = await _repository.ContinueQueryAsync(result.ContinuationToken!, cancellationToken);
            items.AddRange(result.Data);
        }

        if(items.Any())
            await _repository.DeleteAsBatchAsync(items, cancellationToken);

        _ = await _repository.CreateAsync(new Entities.ExampleEntity("Lee"), cancellationToken);
        _ = await _repository.CreateAsync(new Entities.ExampleEntity("Liam"), cancellationToken);
        _ = await _repository.CreateAsync(new Entities.ExampleEntity("Hassan"), cancellationToken);
        _ = await _repository.CreateAsync(new Entities.ExampleEntity("Greg"), cancellationToken);
    }

    public UatExampleEntitySeed(IRepository<Entities.ExampleEntity> repository)
    {
        _repository = repository;
    }
}
