namespace Ilmhub.Judge.Abstractions.Models;

public interface IJudgeResult
{
    bool IsSuccess { get; }
    EJudgeStatus Status { get; }
    ICompilationResult Compilation { get; }
    IEnumerable<ITestCaseResult> TestCases { get; }
}