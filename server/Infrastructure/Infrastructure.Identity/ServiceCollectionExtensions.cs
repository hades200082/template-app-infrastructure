using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Infrastructure.Identity;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        var identityConfig = configuration.GetSection("Identity");

        // If we don't have the "Identity" node in settings then skip this
        if (!identityConfig.Exists()) return services;

        services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddOpenIdConnect(options =>
            {
                // Now we can set any config we want in appSettings such as:
                // - Authority string (url)
                // - ClientId string
                // - ClientSecret string
                // - Scope string[]
                identityConfig.Bind(options);

                // We can also do overrides/additional config here if required

                if (!options.Scope.Any(x => x.Equals("openid", StringComparison.Ordinal)))
                {
                    options.Scope.Add("openid");
                }

                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;

                options.ResponseType = OpenIdConnectResponseType.Code;
                options.ResponseMode = OpenIdConnectResponseMode.Query;
                options.CallbackPath = "/auth/callback";
                options.TokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(5);
            })
            .AddCookie(options =>
            {
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    // Return an Access Denied code rather than the Cookie default of /Account/AccessDenied page
                    context.HttpContext.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });


        return services;

    }
}
