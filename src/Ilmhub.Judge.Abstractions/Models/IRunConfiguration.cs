namespace Ilmhub.Judge.Abstractions.Models;

public interface IRunConfiguration
{
    string ExecutableName { get; set; }
    string Command { get; set; }
    string SeccompRule { get; set; }
    bool MemoryLimitCheckOnly { get; set; }
    IEnumerable<string> EnvironmentVariables { get; set; }
    IEnumerable<string> Arguments { get; set; }
}