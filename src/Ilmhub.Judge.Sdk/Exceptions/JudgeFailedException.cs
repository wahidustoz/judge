namespace Ilmhub.Judge.Sdk.Exceptions;

public class JudgeFailedException : Exception
{
    public JudgeFailedException(string message, Exception innerException) : base(message, innerException) { }
} 