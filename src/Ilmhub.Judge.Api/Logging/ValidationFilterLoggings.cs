namespace Ilmhub.Judge.Api.Logging;
public static partial class ValidationFilterLoggings
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Trace,
        Message = "Fluent validation started for type {targetType}.")]
    public static partial void LogValidationStarted(
        this ILogger logger,
        string targetType);

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Trace,
        Message = "Fluent validation for {targetType} is successful.")]
    public static partial void LogValidationCompleted(
        this ILogger logger,
        string targetType);

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Trace,
        Message = "Fluent validation failed for {targetType} with error: {errors}.")]
    public static partial void LogValidationFilterFailedException(
        this ILogger logger,
        string targetType,
        Dictionary<string, string> errors);

    [LoggerMessage(
        EventId = 5,
        Level = LogLevel.Warning,
        Message = "Error occured during fluent validation for {targetType}.")]
    public static partial void LogValidationException(
        this ILogger logger,
        string targetType,
        Exception exception);
}