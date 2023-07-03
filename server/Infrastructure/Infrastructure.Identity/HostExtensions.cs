using Microsoft.AspNetCore.Builder;

namespace Infrastructure.Identity;

public static class HostExtensions
{
    public static void UseCustomAuthentication(this WebApplication app)
    {
        app.UseAuthorization(); // No longer need UseAuthentication() in .Net 7+

        // Configure any authorization policies here
    }
}
