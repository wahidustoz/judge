using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Abstractions;

public interface ICompiler
{
    ValueTask<ICompilationResult> CompileAsync(
        string source, 
        int languageId,
        string executableFilePath = default,
        CancellationToken cancellationToken = default);
}