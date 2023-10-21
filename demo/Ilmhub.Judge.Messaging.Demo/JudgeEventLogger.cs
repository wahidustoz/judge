using Ilmhub.Judge.Messaging.Shared.Interfaces;

namespace Ilmhub.Judge.Messaging.Demo;

public static partial class JudgeEventLogger
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Receive event {type}: {value}")]
    public static partial void LogJudgeEvent(this ILogger logger, string type, IJudgeEvent value);
}