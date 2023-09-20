using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Models;

public class JudgeRequest : IJudgeRequest
{
    public string SourceCode { get; set; }
    public long MaxCpuTime { get; set; }
    public long MaxMemory { get; set; }
    public bool ShouldReturnOutput { get; set; }
    public ILanguageConfiguration LanguageConfiguration { get; set; }
    public IEnumerable<ITestCase> TestCases { get; set; }
}