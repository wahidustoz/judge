namespace Ilmhub.Judge.Client.Abstractions.Models;

public interface IRunConfiguration
{
    string ExecutableName { get; set; }
    string Command { get; set; }
    string SeccompRule { get; set; }
    IEnumerable<string> Environment { get; set; }
    bool MemoryLimitCheckOnly { get; set; }
}

