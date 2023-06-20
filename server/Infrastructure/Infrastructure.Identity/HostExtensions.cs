using Microsoft.AspNetCore.Builder;

namespace Infrastructure.Identity;

public static class HostExtensions
{
    public static void UseIdentity(this WebApplication app)
    {
        if(app.Configuration.GetSection("Identity").GetChildren().Count() == 1)
            app.UseAuthentication();
    }
}
