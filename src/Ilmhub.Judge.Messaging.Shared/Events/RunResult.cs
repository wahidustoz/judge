using Ilmhub.Judge.Messaging.Shared.Interfaces;

namespace Ilmhub.Judge.Messaging.Shared.Events;

public record RunResult : IRunResult
{
    public string Output { get; set; }
    public string OutputMd5 { get; set; }
    public long? CpuTime { get; set; }
    public long? RealTime { get; set; }
    public long? Memory { get; set; }
}