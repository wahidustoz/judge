namespace Ilmhub.Judge.Sdk.Abstractions.Models;

public interface IJudgeRequest
{
    string SourceCode { get; set; }
    long MaxCpuTime { get; set; }
    long MaxMemory { get; set; }
    bool ShouldReturnOutput { get; set; }
    IEnumerable<ITestCase> TestCases { get; set; }
    ILanguageConfiguration LanguageConfiguration { get; set; }
}