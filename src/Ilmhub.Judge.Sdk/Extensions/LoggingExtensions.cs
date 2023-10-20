using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Ilmhub.Judge.Sdk;

public static partial class LoggingExtensions {
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Warning,
        Message = "JUDGE CLIENT: failed to execute operation {operationName}")]
    public static partial void LogException(this ILogger logger, Exception ex, [CallerMemberName] string operationName = null);
}