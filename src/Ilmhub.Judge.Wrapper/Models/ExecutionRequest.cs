using Ilmhub.Judge.Wrapper.Abstractions.Models;

namespace Ilmhub.Judge.Wrapper.Models;

public class ExecutionRequest : IExecutionRequest
{
    public string ExecutablePath { get; set; }
    public string InputPath { get; set; }
    public string OutputPath { get; set; }
    public string SeccompRuleName { get; set; }
    public IEnumerable<string> Arguments { get; set; }
    public IEnumerable<string> Environments { get; set; }
    public bool MemoryLimitCheckOnly { get; set; }
    public long CpuTime { get; set; } = -1;     // means infinite
    public long RealTime { get; set; } = -1;    // means infinite
    public long Memory { get; set; } = -1;  // means infinite
    public long Stack { get; set; } = -1;   // means infinite
    public long ProcessNumber { get; set; } = -1;   // means infinite
    public long OutputSize { get; set; } = -1;  // means infinite
    public long Uid { get; set; } = 0;          // root userId
    public long Gid { get; set; } = 0;          // root userId
}