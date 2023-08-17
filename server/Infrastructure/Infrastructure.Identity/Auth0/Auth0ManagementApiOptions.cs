using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Identity.Auth0;

public sealed class Auth0ManagementApiOptions
{
    public const string ConfigurationSectionName = "Auth0ManagementApiOptions";

    [Required]
    public string? ClientId { get; init; }

    [Required]
    public string? ClientSecret { get; init; }

    [Required]
    public string? Domain { get; init; }
}
