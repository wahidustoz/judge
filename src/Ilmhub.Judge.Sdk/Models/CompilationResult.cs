using System.Diagnostics;
using Ilmhub.Judge.Sdk.Abstractions.Models;
using Ilmhub.Judge.Wrapper.Abstractions.Models;

namespace Ilmhub.Judge.Sdk;

public class CompilationResult : ICompilationResult
{
    public CompilationResult(IExecutionResult executionResult)
        => Execution = executionResult;
    
    public IExecutionResult Execution { get; set; }
    public string ExecutableFilePath { get; set; }
    public string Error { get; set; }
    public string Output { get; set; }
    public string Log { get; set; }
    public string PotentialWarning => 
        IsSuccess ? Output + Error : string.Empty;
    public bool IsSuccess => Execution.IsSuccess;
}
