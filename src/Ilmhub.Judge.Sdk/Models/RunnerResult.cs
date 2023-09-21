using Ilmhub.Judge.Sdk.Abstraction.Models;
using Ilmhub.Judge.Wrapper.Abstractions.Models;
using Ilmhub.Utilities;

namespace Ilmhub.Judge.Sdk;

public class RunnerResult : IRunnerResult
{
    public RunnerResult(IExecutionResult execution, string output, string error, string log)
    {
        Execution = execution;
        Output = output;
        Error = error;
        Log = log;
    }

    public IExecutionResult Execution { get; }
    public string OutputMd5 => Output.Md5();
    public string Output { get; }
    public string Log { get; }
    public string Error { get; }
    public bool IsSuccess => 
        Execution.Status is EExecutionResult.Success &&
        Execution.Error is EExecutionError.NoError;
}
