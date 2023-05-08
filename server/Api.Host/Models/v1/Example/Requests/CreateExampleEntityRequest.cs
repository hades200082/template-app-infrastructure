using System.ComponentModel.DataAnnotations;

namespace Api.Host.Models.v1.Example.Requests;

public sealed class CreateExampleEntityRequest
{
    public CreateExampleEntityRequest(string name)
    {
        Name = name;
    }

    [Required]
    public string Name { get; }
}
