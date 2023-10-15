namespace Ilmhub.Judge.Sdk.Messaging;

[Serializable]
public class FailedToPublishJudgeCommandException : Exception
{
    public FailedToPublishJudgeCommandException(Exception innerException = null)
        : base("Failed to send judge command.", innerException) { }
}
