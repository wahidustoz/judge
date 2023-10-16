using Ilmhub.Judge.Wrapper.Abstractions.Models;

namespace Ilmhub.Judge.Abstractions.Models;

public interface ICompilationResult
{
    string ExecutableFilePath { get; }
    string Output { get; set; }
    string Log { get; set; }
    string Error { get; set; }
    bool IsSuccess { get; }
    IExecutionResult Execution { get; set; }
}
