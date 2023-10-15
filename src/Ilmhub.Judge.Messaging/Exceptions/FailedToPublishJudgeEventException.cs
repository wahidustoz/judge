namespace Ilmhub.Judge.Messaging;

[Serializable]
public class FailedToPublishJudgeEventException : Exception
{
    public FailedToPublishJudgeEventException(Type eventType, Exception innerException = null)
        : base("Failed to send judge event.", innerException) => EventType = eventType;

    public Type EventType { get; }
}
