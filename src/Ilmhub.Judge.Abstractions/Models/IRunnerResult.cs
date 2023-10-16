using Ilmhub.Judge.Wrapper.Abstractions.Models;

namespace Ilmhub.Judge.Abstractions.Models;

public interface IRunnerResult
{
    string Output { get; }
    string OutputMd5 { get; }
    string Log { get; }
    string Error { get; }
    bool IsSuccess { get; }
    IExecutionResult Execution { get; }
}