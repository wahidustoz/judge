using Ilmhub.Judge.Abstractions.Models;

namespace Ilmhub.Judge.Abstractions;

public interface ICompilationHandler
{
    ValueTask<ICompilationResult> CompileAsync(
        string source,
        int languageId,
        string environmentFolder = default,
        CancellationToken cancellationToken = default);
}