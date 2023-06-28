using Infrastructure.Cosmos;

namespace Domain.DataSeeds.ExampleEntity;

public sealed class DevExampleEntitySeed : ICosmosDataSeed
{
    private readonly IRepository<Entities.ExampleEntity> _repository;
    public int Priority => 0;
    public string TargetEnvironment => "Development";
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Delete anything that already exists
        var result = await _repository.QueryAsync(cancellationToken);
        var items = result.Data.Select(x => x.Id).ToList();
        if (result.HasMore)
        {
            result = await _repository.ContinueQueryAsync(result.ContinuationToken!, cancellationToken);
            items.AddRange(result.Data.Select(x => x.Id));
        }

        if(items.Any())
            await _repository.DeleteAsBatchAsync(items, nameof(Entities.ExampleEntity), cancellationToken);

        _ = await _repository.CreateAsync(new Entities.ExampleEntity("Lee"), cancellationToken);
        _ = await _repository.CreateAsync(new Entities.ExampleEntity("Liam"), cancellationToken);
        _ = await _repository.CreateAsync(new Entities.ExampleEntity("Hassan"), cancellationToken);
        _ = await _repository.CreateAsync(new Entities.ExampleEntity("Greg"), cancellationToken);
    }

    public DevExampleEntitySeed(IRepository<Entities.ExampleEntity> repository)
    {
        _repository = repository;
    }
}
