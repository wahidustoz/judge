namespace Ilmhub.Judge.Sdk.Models;

public record JudgeResult(
    bool IsSuccess,
    EJudgeStatus Status,
    Compilation Compilation,
    IEnumerable<TestCaseExecution> TestCases);