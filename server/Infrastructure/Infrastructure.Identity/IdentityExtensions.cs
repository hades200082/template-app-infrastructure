using System.Security.Claims;

namespace Infrastructure.Identity;

public static class IdentityExtensions
{
    /// <summary>
    /// Attempts to returns the "subject" claim from the logged in user.
    /// Based on the JWT RFC, this should contain the identity provider's
    /// UserID for this user.
    /// </summary>
    /// <param name="identity"></param>
    /// <returns></returns>
    public static string? ExternalId(this ClaimsPrincipal identity)
    {
        return identity.FindFirst("sub")?.Value;
    }
}
