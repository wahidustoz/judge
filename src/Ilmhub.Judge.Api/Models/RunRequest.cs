namespace Ilmhub.Judge.Api.Models;

public class RunRequest
{
    public string ExecutablePath { get; set; }
    public string InputPath { get; set; }
    public string OutputPath { get; set; }
    public string SeccompRuleName { get; set; }
    public IEnumerable<string> Arguments { get; set; }
    public IEnumerable<string> Environments { get; set; }
    public bool MemoryLimitCheckOnly { get; set; }
    public long CpuTime { get; set; }
    public long RealTime { get; set; }
    public long Memory { get; set; }
    public long Stack { get; set; }
    public long ProcessNumber { get; set; }
    public long OutputSize { get; set; }
    public long Uid { get; set; }
    public long Gid { get; set; }
}
