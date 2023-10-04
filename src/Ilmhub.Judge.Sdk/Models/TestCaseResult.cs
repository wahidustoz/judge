using Ilmhub.Judge.Sdk.Abstraction.Models;
using Ilmhub.Judge.Sdk.Abstractions.Models;
using Ilmhub.Judge.Wrapper.Abstractions.Models;
using Ilmhub.Utilities;

namespace Ilmhub.Judge.Sdk.Models;

public class TestCaseResult : ITestCaseResult
{
    public TestCaseResult(string id, string expectedOutput, IRunnerResult execution, bool? usestrictMode = default)
    {
        Id = id;
        ExpectedOutput = expectedOutput;
        Execution = execution;
        UseStrictMode = usestrictMode;
    }

    public string Id { get; }
    public string ExpectedOutput { get; }
    public string ExpectedOutputMd5 => ExpectedOutput.Md5();
    public string OutputMd5 => Execution.OutputMd5;
    public string Output => Execution.Output;
    public IRunnerResult Execution { get; }
    public ETestCaseStatus Status => GetStatus();
    public bool? UseStrictMode { get; }

    private ETestCaseStatus GetStatus() => Execution.IsSuccess switch
    {
        true when UseStrictMode is true && string.Equals(OutputMd5, ExpectedOutputMd5, StringComparison.OrdinalIgnoreCase) 
            => ETestCaseStatus.Success,
        true when (UseStrictMode is false || UseStrictMode is null) && string.Equals(Output.Trim().Md5(), ExpectedOutput.Trim().Md5(), StringComparison.OrdinalIgnoreCase)
            => ETestCaseStatus.Success,
        true when string.Equals(Output.Trim().Md5(), ExpectedOutput.Trim().Md5(), StringComparison.OrdinalIgnoreCase) 
            => ETestCaseStatus.PresentationError,
        true => ETestCaseStatus.WrongAnswer,
        false when Execution.Execution.Status is EExecutionResult.CpuTimeLimitExceeded
            => ETestCaseStatus.CpuLimit,
        false when Execution.Execution.Status is EExecutionResult.MemoryLimitExceeded
            => ETestCaseStatus.MemoryLimit,
        false when Execution.Execution.Status is EExecutionResult.RealTimeLimitExceeded
            => ETestCaseStatus.RealtimeLimit,
        false when Execution.Execution.Status is EExecutionResult.RuntimeError
            => ETestCaseStatus.RuntimeError,
        _ => ETestCaseStatus.SystemError
    };
}