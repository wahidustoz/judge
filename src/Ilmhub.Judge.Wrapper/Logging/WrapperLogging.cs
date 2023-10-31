using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Ilmhub.Judge.Wrapper.Logging;

public static partial class WrapperLogging
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Trace,
        Message = "Wrapper started for request: {request}, requestId: {requestId}")]
    public static partial void LogWrapperStarted(
        this ILogger logger,
        string request,
        long requestId);

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Wrapper completed for request: {request}, requestId: {requestId}")]
    public static partial void LogWrapperCompleted(
        this ILogger logger,
        string request,
        long requestId);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Information,
        Message = "Libjudger.so process finished. Exit code: {exitCode}, Output: {output}, Error: {error}.")]
    public static partial void LogWrapperCompleted(
        this ILogger logger,
        int exitCode,
        string output,
        string error);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Error,
        Message = "JudgeWrapper failed to deserialize Libjudger.so output.")]
    public static partial void LogJudgeWrapperFailedException(
        this ILogger logger,
        JsonException jsonException);

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Warning,
        Message = "JudgeWrapper: Libjudger.so process faild while executing {executable}.")]
    public static partial void LogJudgeWrapperFailedException(
        this ILogger logger,
        string executable,
        Exception ex);
}