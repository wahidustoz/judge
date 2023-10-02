using Ilmhub.Judge.Sdk.Models;

namespace Ilmhub.Judge.Api.Dtos;

public class JudgeRequestDto
{
    public int LanguageId { get; set; }
    public string Source { get; set; }
    public long? MaxCpu { get; set; }
    public long? MaxMemory { get; set; }
    public Guid? TestCaseId { get; set; }
    public IEnumerable<TestCase> TestCases { get; set; }
    public bool? UseStrictMode { get; set; }
}
