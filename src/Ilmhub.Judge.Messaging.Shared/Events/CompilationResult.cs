using Ilmhub.Judge.Messaging.Shared.Interfaces;

namespace Ilmhub.Judge.Messaging.Shared.Events;

public record CompilationResult : ICompilationResult
{
    public bool IsSuccess { get; set; }
    public string Output { get; set; }
    public string Error { get; set; }
    public string Status { get; set; }
}