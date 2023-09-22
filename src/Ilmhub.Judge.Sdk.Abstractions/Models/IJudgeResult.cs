using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Abstractions;

public interface IJudgeResult
{
    bool IsSuccess { get; }
    EJudgeStatus Status { get; }
    ICompilationResult Compilation { get; }
    IEnumerable<ITestCaseResult> TestCases { get; }
}
