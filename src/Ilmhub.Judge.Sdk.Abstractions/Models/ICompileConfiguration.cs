namespace Ilmhub.Judge.Sdk.Abstractions.Models;

public interface ICompileConfiguration
{
    string SourceName { get; set; }
    string DotnetProjectPath { get; set; }
    string ExecutableName { get; set; }
    int MaxCpuTime { get; set; }
    int MaxRealTime { get; set; }
    int MaxMemory { get; set; }
    string Command { get; set; }
    IEnumerable<string> EnvironmentVariables { get; set; }
    IEnumerable<string> Arguments { get; set; }
}