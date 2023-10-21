namespace Ilmhub.Judge.Api.Logging;

public static partial class CommandHandlerLoggings
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Trace, Message = "Started processing {commandType} with request id {requestId}.")]
    public static partial void LogCommandHandlerStarted(
        this ILogger logger,
        string commandType,
        Guid requestId);

    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Completed processing {commandType} with request id {requestId}.")]
    public static partial void LogCommandHandlerCompleted(
        this ILogger logger,
        string commandType,
        Guid requestId);

    [LoggerMessage(EventId = 2, Level = LogLevel.Warning, Message = "Judge request with id {requestId} FAILED at JudgeCommandHandler.")]
    public static partial void LogJudgeFailedException(
        this ILogger<JudgeCommandHandler> logger,
        Exception exception,
        Guid requestId);

    [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "Run request with id {requestId} FAILED at RunCommandHandler.")]
    public static partial void LogRunFailedException(
        this ILogger<RunCommandHandler> logger,
        Exception exception,
        Guid requestId);
}