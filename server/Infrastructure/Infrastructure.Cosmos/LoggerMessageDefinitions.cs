using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Cosmos;

internal static class LoggerMessageDefinitions
{
    private static readonly Action<ILogger, string, double, CosmosDiagnostics, Exception?> s_logCosmosDiagnosticsTraceDefinition =
        LoggerMessage.Define<string, double, CosmosDiagnostics>(LogLevel.Trace, 0,
            "{Query} completed using {RequestCharge} RUs. Diagnostics: {Diagnostics}");

    public static void LogCosmosDiagnosticsTrace(this ILogger logger, string query, double requestCharge, CosmosDiagnostics diagnostics)
    {
        s_logCosmosDiagnosticsTraceDefinition(logger, query, requestCharge, diagnostics, null);
    }

    private static readonly Action<ILogger, string, object, Exception?> s_logCosmosExceptionDefinition =
        LoggerMessage.Define<string, object>(LogLevel.Error, 0,
            "{Method} failed querying [{MethodArguments}]");

    public static void LogCosmosException(this ILogger logger, object methodArguments, CosmosException cex, [System.Runtime.CompilerServices.CallerMemberName] string method = "")
    {
        s_logCosmosExceptionDefinition(logger, method, methodArguments, cex);
    }
}
