using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Models;

public class CompileConfiguration : ICompileConfiguration
{
    public string SourceName { get; set; }
    public string DotnetProjectPath { get; set; }
    public string ExecutableName { get; set; }
    public int MaxCpuTime { get; set; } = -1;       // means infinite
    public int MaxRealTime { get; set; } = -1;      // means infinite
    public int MaxMemory { get; set; } = -1;        // means infinite
    public string Command { get; set; }
    public IEnumerable<string> EnvironmentVariables { get; set; }
    public IEnumerable<string> Arguments { get; set; }
}
