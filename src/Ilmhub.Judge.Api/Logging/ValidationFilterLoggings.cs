namespace Ilmhub.Judge.Api.Logging;
public static partial class ValidationFilterLoggings
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Trace,
        Message = "FluentAsynValidationFilter started for type {targetType}.")]
    public static partial void LogValidationFilterStarted(
        this ILogger logger,
        string targetType);

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Trace,
        Message = "FluentAsyncValidationFilter validated {targetType} successfully.")]
    public static partial void LogValidationFilterCompleted(
        this ILogger logger,
        string targetType);

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Trace,
        Message = "Fluent validation failed for {targetType} with error: {errors}.")]
    public static partial void LogValidationFilterValidationResulProblem(
        this ILogger logger,
        string targetType,
        Dictionary<string, string> errors);

    [LoggerMessage(
        EventId = 5,
        Level = LogLevel.Warning,
        Message = "FluentAsynValidationFilter Validation failed {validationFilter}")]
    public static partial void LogValidationFilterFailedException(
        this ILogger logger,
        string validationFilter,
        Exception exception);
}