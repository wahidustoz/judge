namespace Ilmhub.Judge.Wrapper.Exceptions;

public class JudgeProcessFailedException : Exception
{
    public JudgeProcessFailedException(string message, Exception innerException)
        : base(message, innerException: innerException) { }
}