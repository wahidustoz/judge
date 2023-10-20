namespace Ilmhub.Judge.Sdk.Models;

public record RunResult(
    bool IsSuccess,
    Compilation Compilation,
    IEnumerable<TestCaseExecution> TestCases);