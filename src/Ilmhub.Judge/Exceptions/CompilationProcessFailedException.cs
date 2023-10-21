namespace Ilmhub.Judge.Exceptions;

public class CompilationProcessFailedException : Exception
{
    public CompilationProcessFailedException(string message, Exception innerException)
        : base(message, innerException) { }
}