namespace Ilmhub.Judge.Sdk.Abstractions.Models;

public interface IJudgeRequest
{
    ILanguageConfiguration LanguageConfiguration { get; set; }
    IEnumerable<ITestCase> TestCases { get; set; }
    string TestCaseId { get; set; }
    string SourceCode { get; set; }
    long MaxCpuTime { get; set; }
    long MaxMemory { get; set; }
    bool ShouldReturnOutput { get; set; }
}