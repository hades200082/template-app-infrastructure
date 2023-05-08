namespace Application.DtoModels;

public sealed class ExampleEntityDto
{
    public ExampleEntityDto(string id, string partitionKey, string name)
    {
        Id = id;
        PartitionKey = partitionKey;
        Name = name;
    }

    public string Id { get; init; }
    public string PartitionKey { get; init; }
    public string Name { get; init; }
}
