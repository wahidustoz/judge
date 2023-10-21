using Ilmhub.Judge.Abstractions.Models;

namespace Ilmhub.Judge.Models;

public class RunConfiguration : IRunConfiguration
{
    public string ExecutableName { get; set; }
    public string Command { get; set; }
    public string SeccompRule { get; set; }
    public bool MemoryLimitCheckOnly { get; set; }
    public IEnumerable<string> EnvironmentVariables { get; set; }
    public IEnumerable<string> Arguments { get; set; }
}