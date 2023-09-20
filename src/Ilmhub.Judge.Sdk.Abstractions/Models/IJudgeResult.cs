namespace Ilmhub.Judge.Sdk.Abstractions.Models;

public interface IJudgeResult
{
    IEnumerable<ITestCaseResult> TestCases { get; set; }
}
