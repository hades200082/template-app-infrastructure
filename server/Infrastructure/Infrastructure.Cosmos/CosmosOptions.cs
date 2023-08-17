using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Cosmos;

public sealed class CosmosOptions
{
    public const string ConfigurationSectionName = "CosmosOptions";

    [Required]
    public string? AccountEndpoint { get; set; }
    [Required]
    public string? AccountKey { get; set; }
    [Required]
    public string? DatabaseId { get; set; }
    [Required]
    public string? ContainerName { get; set; }
}
