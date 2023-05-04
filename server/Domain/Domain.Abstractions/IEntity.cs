namespace Domain.Abstractions;

public interface IEntity
{
    string Id { get; init; }
    string Type { get; }
    string PartitionKey { get; }
}