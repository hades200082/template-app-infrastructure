using Microsoft.Extensions.Logging;

namespace Shared.Core;

public static class LoggerMessageDefinitions
{
    private static readonly Action<ILogger, string, object?, Exception?> s_logMethodCallDefinition =
        LoggerMessage.Define<string, object?>(LogLevel.Trace, 0,
            "{Method} called for [{MethodArguments}]");

    public static void LogMethodCall(this ILogger logger, object? methodArguments, [System.Runtime.CompilerServices.CallerMemberName] string method = "")
    {
        s_logMethodCallDefinition(logger, method, methodArguments, null);
    }


    private static readonly Action<ILogger, string, Exception?> s_logSimpleTrace =
        LoggerMessage.Define<string>(LogLevel.Trace, 0,
            "{Message}");

    public static void LogSimpleTrace(this ILogger logger, string message)
    {
        s_logSimpleTrace(logger, message, null);
    }
}
