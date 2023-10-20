namespace Ilmhub.Judge.Api.Logging;
public static partial class ValidationFilterLoggings
{
    [LoggerMessage(
        EventId = 0, 
        Level = LogLevel.Trace, 
        Message = "FluentAsynValidationFilter started {validationFilter} with request Body {body}")]
    public static partial void LogValidationFilterStarted(
        this ILogger logger,
        string validationFilter,
        Stream body);

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "FluentAsynValidationFilter executed successfully with {validationFilter}"
    )]
    public static partial void LogValidationFilterCompleted(
        this ILogger logger,
        string validationFilter
    );

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Warning,
        Message = "FluentAsynValidationFilter argument is null with {validationFilter}"
    )]
    public static partial void LogValidationFilterArgumentOrNull(
        this ILogger logger,
        string validationFilter
    );

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Trace,
        Message = "FluentAsynValidationFilter Validation started {validationFilter}"
    )]
    public static partial void LogValidationFilterValidationStarted(
        this ILogger logger,
        string validationFilter
    );

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Trace,
        Message = "FluentAsynValidationFilter Validation IsValid false {validationFilter} with {keyValuePairs} statusCode {statusCode}"
    )]
    public static partial void LogValidationFilterValidationResulProblem(
        this ILogger logger,
        string validationFilter,
        Dictionary<string, string> keyValuePairs,
        int statusCode
    );

    [LoggerMessage(
        EventId = 5,
        Level = LogLevel.Warning,
        Message = "FluentAsynValidationFilter Validation failed {validationFilter}"
    )]
    public static partial void LogValidationFilterFailedException(
        this ILogger logger,
        string validationFilter,
        Exception exception
    );
}