namespace Ilmhub.Judge.Wrapper.Abstractions.Models;

public interface IExecutionResult
{
    string Output { get; set; }
    string ErrorMessage { get; set; }
    int CpuTime { get; set; }
    int RealTime { get; set; }
    long Memory { get; set; }
    int Signal { get; set; }
    int ExitCode { get; set; }
    EExecutionError Error { get; set; }
    EExecutionResult Status { get; set; }
}

