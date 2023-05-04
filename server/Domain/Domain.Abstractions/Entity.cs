using Newtonsoft.Json;

namespace Domain.Abstractions;

public abstract class Entity : IEntity
{
    public string Id { get; init; } = Guid.NewGuid().ToString();

    public string Type => GetType().Name;

    [JsonProperty("_pk")]
    public virtual string PartitionKey => Type;
}