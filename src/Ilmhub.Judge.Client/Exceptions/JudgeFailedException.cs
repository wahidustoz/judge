namespace Ilmhub.Judge.Sdk.Exceptions;

[Serializable]
public class JudgeFailedException : Exception
{
    public JudgeFailedException(string message) : base(message) { }
}