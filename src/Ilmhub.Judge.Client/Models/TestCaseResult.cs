using Ilmhub.Judge.Client.Abstractions.Models;

namespace Ilmhub.Judge.Client.Models;

public class TestCaseResult : ITestCaseResult
{
    public long CpuTime { get; set; }
    public long RealTime { get; set; }
    public long Memory { get; set; }
    public long Signal { get; set; }
    public long ExitCode { get; set; }
    public long Error { get; set; }
    public EJudgeStatus Status { get; set; }
    public string TestCase { get; set; }
    public string OutputMd5 { get; set; }
    public string Output { get; set; }
}