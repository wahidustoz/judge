namespace Ilmhub.Judge.Sdk;

public class CompilationProcessFailedException : Exception
{
    public CompilationProcessFailedException(string message, Exception innerException)
        : base(message, innerException) { }
}
