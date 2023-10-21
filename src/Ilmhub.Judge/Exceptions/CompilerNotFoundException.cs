namespace Ilmhub.Judge.Exceptions;

public class CompilerNotFoundException : Exception
{
    public CompilerNotFoundException(int languageId)
        : base($"Compiler for language {languageId} not found") { }
}