using Ilmhub.Judge.Client.Abstractions.Models;

namespace Ilmhub.Judge.Client.Models;

public class JudgeResult : IJudgeResult
{
    public string ErrorMessage { get; set; }
    public bool IsSuccess => string.IsNullOrWhiteSpace(ErrorMessage) && TestCases.Any();
    public IEnumerable<ITestCaseResult> TestCases { get; set; }
}