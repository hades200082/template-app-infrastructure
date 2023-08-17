using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Identity.Auth0;

public sealed class Auth0Options
{
    public const string ConfigurationSectionName = "Auth0Options";

    [Required]
    public string? Domain { get; init; }

    public string? NameClaimType { get; init; }

    [Required]
    [MinLength(1)]
    public string[]? ValidAudiences { get; init; }

}
