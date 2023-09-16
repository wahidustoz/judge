using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Models;

public class CompileError : ICompileError
{
    public string Message { get; set; }
    public ECompileErrorStatus Status { get; set; }
    public long CpuTime { get; set; }
    public long RealTime { get; set; }
    public long Memory { get; set; }
    public long Signal { get; set; }
    public long ExitCode { get; set; }
    public long Error { get; set; }
}