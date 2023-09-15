namespace Ilmhub.Judge.Client.Abstractions.Models;

public interface ITestCaseResult
{
    long CpuTime { get; set; }
    long RealTime { get; set; }
    long Memory { get; set; }
    long Signal { get; set; }
    long ExitCode { get; set; }
    long Error { get; set; }
    long Result { get; set; }
    string TestCase { get; set; }
    string OutputMd5 { get; set; }
    string Output { get; set; }
}