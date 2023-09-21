using Ilmhub.Judge.Wrapper.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Abstractions.Models;

public interface ICompilationResult
{
    string ExecutableFilePath { get; }
    string Output { get; set; }
    string Log { get; set; }
    string Error { get; set; }
    string PotentialWarning { get; }
    bool IsSuccess { get; }
    IExecutionResult Execution { get; set; }
}
