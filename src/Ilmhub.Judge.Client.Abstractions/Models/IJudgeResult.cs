namespace Ilmhub.Judge.Client.Abstractions.Models;

public interface IJudgeResult
{
    string ErrorMessage { get; set; }
    bool IsSuccess { get; }
    IEnumerable<ITestCaseResult> TestCases { get; set; }
}
