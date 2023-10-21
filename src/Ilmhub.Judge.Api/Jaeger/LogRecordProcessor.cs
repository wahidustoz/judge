using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Logs;

namespace Ilmhub.Judge.Api.Jaeger;

public class LogRecordProcessor : BaseProcessor<LogRecord>
{
    private readonly LogRecordProcessorOptions options;

    public LogRecordProcessor(LogRecordProcessorOptions options) => this.options = options ?? new LogRecordProcessorOptions();

    public override void OnEnd(LogRecord data)
    {
        if (Activity.Current != null)
        {
            var tags = new ActivityTagsCollection
                {
                    { nameof(data.CategoryName), data.CategoryName },
                    { nameof(data.EventId), data.EventId },
                    { nameof(data.Exception), data.Exception },
                    { nameof(data.LogLevel), data.LogLevel },
                    { nameof(data.SpanId), data.SpanId },
                    { nameof(data.Attributes), data.Attributes },
                    { nameof(data.Timestamp), data.Timestamp },
                    { nameof(data.TraceId), data.TraceId },
                    { nameof(data.TraceState), data.TraceState },
                    { nameof(data.FormattedMessage), data.FormattedMessage }
                };

            var activityEvent = new ActivityEvent(data.CategoryName, default, tags);
            Activity.Current.AddEvent(activityEvent);
        }
    }
}