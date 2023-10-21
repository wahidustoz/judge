using Ilmhub.Judge.Abstractions.Models;
using Ilmhub.Judge.Wrapper.Abstractions.Models;
using Ilmhub.Utilities;

namespace Ilmhub.Judge.Models;

public class TestCaseResult : ITestCaseResult
{
    private readonly bool? useStrictMode;
    public TestCaseResult(string id, string expectedOutput, IRunnerResult execution, bool? usestrictMode = default)
    {
        Id = id;
        ExpectedOutput = expectedOutput;
        Execution = execution;
        useStrictMode = usestrictMode;
    }

    public string Id { get; }
    public string ExpectedOutput { get; }
    public string ExpectedOutputMd5 => ExpectedOutput.Md5();
    public string OutputMd5 => Execution.OutputMd5;
    public string Output => Execution.Output;
    public IRunnerResult Execution { get; }
    public ETestCaseStatus Status => GetStatus();

    private ETestCaseStatus GetStatus() => Execution.IsSuccess switch
    {
        true when OutputMatches(strict: useStrictMode)
            => ETestCaseStatus.Success,
        true when useStrictMode is true && OutputMatches(strict: false)
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

    private bool OutputMatches(bool? strict)
        => strict is true
        ? string.Equals(Output.Md5(), ExpectedOutputMd5)
        : string.Equals(Output.Trim().Md5(), ExpectedOutput.Trim().Md5());
}