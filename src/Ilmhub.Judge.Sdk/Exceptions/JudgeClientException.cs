namespace Ilmhub.Judge.Sdk.Exceptions;

[Serializable]
public class JudgeClientException : Exception {
    public JudgeClientException(Exception innerException)
        : base("Judge Client request failed.", innerException) { }
}