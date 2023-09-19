namespace Ilmhub.Judge.Wrapper.Abstractions.Models;

public interface IExecutionResult
{
    public string ErrorMessage { get; set; }
    public bool IsSuccess => string.IsNullOrWhiteSpace(ErrorMessage);
    public int CpuTime { get; set; }
    public int RealTime { get; set; }
    public long Memory { get; set; }
    public int Signal { get; set; }
    public int ExitCode { get; set; }
    public EExecutionError Error { get; set; }
    public EExecutionResult Status { get; set; }
}

