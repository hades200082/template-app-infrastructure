namespace Application.MessageModels.Events;

public sealed class ExampleEntityCreated
{
    public ExampleEntityCreated(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Id { get; set; }
    public string Name { get; set; }
}
