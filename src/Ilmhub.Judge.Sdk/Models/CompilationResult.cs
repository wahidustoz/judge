using System.Diagnostics;
using Ilmhub.Judge.Sdk.Abstractions.Models;
using Ilmhub.Judge.Wrapper.Abstractions.Models;

namespace Ilmhub.Judge.Sdk;

public class CompilationResult : ICompilationResult
{
    public CompilationResult(IExecutionResult executionResult)
        => Execution = executionResult;
    // TODO: implement this after tests
    public ECompilationStatus Status { get; set; }
    public IExecutionResult Execution { get; set; }
}
