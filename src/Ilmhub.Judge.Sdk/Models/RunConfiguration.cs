using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Models;

public class RunConfiguration : IRunConfiguration
{
    public string ExecutableName { get; set; }
    public string Command { get; set; }
    public string SeccompRule { get; set; }
    public IEnumerable<string> Environment { get; set; }
    public bool MemoryLimitCheckOnly { get; set; }
}
