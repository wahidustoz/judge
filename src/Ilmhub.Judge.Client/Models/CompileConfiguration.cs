using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Models;

public class CompileConfiguration : ICompileConfiguration
{
    public string SourceName { get; set; }
    public string ExecutableName { get; set; }
    public int MaxCpuTime { get; set; }
    public int MaxRealTime { get; set; }
    public int MaxMemory { get; set; }
    public string CompileCommand { get; set; }
    public IEnumerable<string> Environment { get; set; }
}
