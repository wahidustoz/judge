namespace Ilmhub.Judge.Client.Abstractions.Models;

public interface ICompileError
{
    string Message { get; set; }
    ECompileErrorStatus Status { get; set; }
    long CpuTime { get; set; }
    long RealTime { get; set; }
    long Memory { get; set; }
    long Signal { get; set; }
    long ExitCode { get; set; }
    long Error { get; set; }
}