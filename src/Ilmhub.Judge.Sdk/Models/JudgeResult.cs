using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Models;

public class JudgeResult : IJudgeResult
{
    public IEnumerable<ITestCaseResult> TestCases { get; set; }
}