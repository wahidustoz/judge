namespace Ilmhub.Judge.Messaging.Shared.Interfaces;

public interface IJudgeCompleted : IJudgeEvent
{
    string Status { get; set; }
    ICompilationResult CompilationResult { get; set; }
    IEnumerable<ITestCaseResult> TestCases { get; set; }
}