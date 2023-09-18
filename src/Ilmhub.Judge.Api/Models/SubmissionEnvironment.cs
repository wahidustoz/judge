namespace Ilmhub.Judge.Api.Models;

public class SubmissionEnvironment
{
    public string SourcePath { get; set; }
    public string TestcasesPath { get; set; }
    public string ExecutablePath { get; set; }
    public Dictionary<string, string> Testcases { get; set; } = new();
}