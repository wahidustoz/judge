using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Exceptions;

[Serializable]
public class CompileErrorException : Exception
{
    public CompileErrorException(ICompileError error)
        : base($"Compile error occured: {error.Message}")
        => Error = error;

    public ICompileError Error { get; }
}
