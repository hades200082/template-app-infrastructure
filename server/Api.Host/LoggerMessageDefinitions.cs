using System.Security.Claims;

namespace Api.Host;

public static class LoggerMessageDefinitions
{
    private static readonly Action<ILogger, string, string, object?, Exception?> s_logControllerRequestTrace =
        LoggerMessage.Define<string, string, object?>(LogLevel.Trace, 0,
            "{Controller}/{Action} hit with [{RouteData}]");

    public static void LogControllerRequestTrace(this ILogger logger, object? methodArguments, [System.Runtime.CompilerServices.CallerFilePath] string controller = "", [System.Runtime.CompilerServices.CallerMemberName] string action = "")
    {
        s_logControllerRequestTrace(logger, controller, action, methodArguments, null);
    }

    private static readonly Action<ILogger, ClaimsPrincipal?, Exception?> s_logNoExternalIdError =
        LoggerMessage.Define<ClaimsPrincipal?>(LogLevel.Error, 0,
            "Authenticated request contained no external user ID. {ClaimsPrincipal}");

    public static void LogExternalIdError(this ILogger logger, ClaimsPrincipal user)
    {
        s_logNoExternalIdError(logger, user, null);
    }
}
