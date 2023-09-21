using Ilmhub.Judge.Wrapper.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Abstraction.Models;

public interface IRunnerResult
{
    string Output { get; set; }
    string OutputMd5 { get; }
    string Log { get; set; }
    string Error { get; set; }
    bool IsSuccess { get; }
    IExecutionResult Execution { get; set; }
}