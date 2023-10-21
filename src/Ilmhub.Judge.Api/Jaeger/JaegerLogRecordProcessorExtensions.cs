using OpenTelemetry.Logs;

namespace Ilmhub.Judge.Api.Jaeger;

public static class JaegerLoggerProcessorExtensions
{
    /// <summary>
    /// Adds Jaeger LogRecord Processor as a configuration to the OpenTelemetry ILoggingBuilder.
    /// </summary>
    /// <param name="loggerOptions"><see cref="OpenTelemetryLoggerOptions"/> options to use.</param>
    /// <param name="configure">Exporter configuration options.</param>
    /// <returns>The instance of <see cref="OpenTelemetryLoggerOptions"/> to chain the calls.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="loggerOptions"/> is <c>null</c>.</exception>
    public static OpenTelemetryLoggerOptions AddLogRecordProcessor(this OpenTelemetryLoggerOptions loggerOptions, Action<LogRecordProcessorOptions> configure = null)
    {
        if (loggerOptions == null)
        {
            throw new ArgumentNullException(nameof(loggerOptions));
        }

        var options = new LogRecordProcessorOptions();
        configure?.Invoke(options);
        return loggerOptions.AddProcessor(new LogRecordProcessor(options));
    }
}