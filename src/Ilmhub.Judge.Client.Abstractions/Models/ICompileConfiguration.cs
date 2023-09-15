namespace Ilmhub.Judge.Client.Abstractions.Models;

public interface ICompileConfiguration
{
    string SourceName { get; set; }
    string ExecutableName { get; set; }
    int MaxCpuTime { get; set; }
    int MaxRealTime { get; set; }
    int MaxMemory { get; set; }
    string CompileCommand { get; set; }
    IEnumerable<string> Environment { get; set; }
}

