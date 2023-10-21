namespace Ilmhub.Judge.Wrapper.Abstractions.Models;

public interface IExecutionRequest
{
    string ExecutablePath { get; set; }
    string InputPath { get; set; }
    string OutputPath { get; set; }
    string ErrorPath { get; set; }
    string LogPath { get; set; }
    string SeccompRuleName { get; set; }
    IEnumerable<string> Arguments { get; set; }
    IEnumerable<string> Environments { get; set; }
    bool MemoryLimitCheckOnly { get; set; }
    long CpuTime { get; set; }
    long RealTime { get; set; }
    long Memory { get; set; }
    long Stack { get; set; }
    long ProcessNumber { get; set; }
    long OutputSize { get; set; }
    long Uid { get; set; }
    long Gid { get; set; }
}