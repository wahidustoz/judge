namespace Ilmhub.Judge.Sdk.Exceptions;

[Serializable]
public class FailedToPublishJudgeCommandException : Exception {
    public FailedToPublishJudgeCommandException(Exception innerException = null)
        : base("Failed to send judge command.", innerException) { }
}