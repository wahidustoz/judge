namespace Ilmhub.Judge.Messaging.Shared.Interfaces;

public interface IRunCompleted : IJudgeEvent
{
    ICompilationResult CompilationResult { get; set; }
    IEnumerable<IRunResult> Outputs { get; set; }
}