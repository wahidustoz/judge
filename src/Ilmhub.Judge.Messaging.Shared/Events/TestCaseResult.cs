using Ilmhub.Judge.Messaging.Shared.Interfaces;

namespace Ilmhub.Judge.Messaging.Shared.Events;
public record TestCaseResult : ITestCaseResult
{
    public string Id { get; set; }
    public string Status { get; set; }
    public string Output { get; set; }
    public string OutputMd5 { get; set; }
    public long? CpuTime { get; set; }
    public long? RealTime { get; set; }
    public long? Memory { get; set; }
}