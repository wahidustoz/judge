namespace Ilmhub.Judge.Messaging.Shared.Interfaces;

public interface IRunResult
{
    string Output { get; set; }
    string OutputMd5 { get; set; }
    long? CpuTime { get; set; }
    long? RealTime { get; set; }
    long? Memory { get; set; }
}