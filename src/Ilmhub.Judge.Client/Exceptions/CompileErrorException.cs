using Ilmhub.Judge.Client.Abstractions.Models;

namespace Ilmhub.Judge.Client;

[Serializable]
public class CompileErrorException : Exception
{
    public CompileErrorException(ICompileError error)
        : base($"Compile error occured: {error.Message}")
        => Error = error;

    public ICompileError Error { get; }
}
