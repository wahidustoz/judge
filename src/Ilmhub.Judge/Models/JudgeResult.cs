using Ilmhub.Judge.Abstractions.Models;

namespace Ilmhub.Judge.Models;

public class JudgeResult : IJudgeResult
{
    public JudgeResult(ICompilationResult compilation) => Compilation = compilation;
    public JudgeResult(ICompilationResult compilation, IEnumerable<ITestCaseResult> testCases)
        : this(compilation) => TestCases = testCases;

    public bool IsSuccess => IsCompilationSuccess(Compilation) && AreTestCasesSuccess(TestCases);
    public ICompilationResult Compilation { get; }
    public IEnumerable<ITestCaseResult> TestCases { get; }
    public EJudgeStatus Status => IsSuccess switch
    {
        true when TestCases?.All(t => t.Status is ETestCaseStatus.Success) is true
            => EJudgeStatus.Accepted,
        true when TestCases?.Any(t => t.Status is ETestCaseStatus.Success) is true
            => EJudgeStatus.PartialAccepted,
        true => EJudgeStatus.NotAccepted,
        false when IsCompilationSuccess(Compilation) is false
            => EJudgeStatus.CompilationError,
        _ => EJudgeStatus.OtherError
    };

    private static bool IsCompilationSuccess(ICompilationResult compilation)
        => compilation is not null && compilation.IsSuccess;
    private static bool AreTestCasesSuccess(IEnumerable<ITestCaseResult> testCases)
        => testCases?.Any() is false || testCases.All(t => t.Execution.IsSuccess);
}