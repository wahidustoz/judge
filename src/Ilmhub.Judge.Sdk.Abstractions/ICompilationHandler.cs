using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Abstractions;

public interface ICompilationHandler
{
    ValueTask<ICompilationResult> CompileAsync(
        string source, 
        int languageId,
        string environmentFolder = default,
        CancellationToken cancellationToken = default);
}