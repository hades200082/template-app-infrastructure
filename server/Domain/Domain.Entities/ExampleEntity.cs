using Domain.Abstractions;

namespace Domain.Entities;

public sealed class ExampleEntity : Entity
{
    public ExampleEntity()
    {
        Name = string.Empty;
    }

    public string Name { get; set; }
}
