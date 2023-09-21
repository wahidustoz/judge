using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Models;

public class JudgeResult : IJudgeResult
{
    public JudgeResult(ICompilationResult compilation) => Compilation = compilation;
    public JudgeResult(ICompilationResult compilation, IEnumerable<ITestCaseResult> testCases)
        : this(compilation) => TestCases = testCases;
        
    public bool IsSuccess => IsCompilationSuccess(Compilation) && AreTestCasesSuccess(TestCases);
    public ICompilationResult Compilation { get; }
    public IEnumerable<ITestCaseResult> TestCases { get; }

    private static bool IsCompilationSuccess(ICompilationResult compilation)
        => compilation is not null && compilation.IsSuccess;
    private static bool AreTestCasesSuccess(IEnumerable<ITestCaseResult> testCases)
        => testCases?.Any() is false || testCases.All(t => t.Execution.IsSuccess);
}
