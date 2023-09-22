namespace Ilmhub.Judge.Wrapper;

public class JudgeProcessFailedException : Exception
{
    public JudgeProcessFailedException(string message, Exception innerException) 
        : base(message, innerException: innerException) { }
}
