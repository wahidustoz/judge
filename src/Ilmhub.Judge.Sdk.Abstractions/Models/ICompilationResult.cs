using Ilmhub.Judge.Wrapper.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Abstractions.Models;

public interface ICompilationResult
{
    ECompilationStatus Status { get; set; }
    IExecutionResult Execution { get; set; }
}
