using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Storage;

public sealed class StorageOptions
{
    [Required]
    public string? AccountKey { get; init; }

    [Required]
    public string? AccountName { get; init; }

    [Required]
    public string? ContainerName { get; init; }
}
