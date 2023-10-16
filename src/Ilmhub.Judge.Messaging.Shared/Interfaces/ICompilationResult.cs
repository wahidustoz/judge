namespace Ilmhub.Judge.Messaging.Shared.Interfaces;

public interface ICompilationResult
{
    bool IsSuccess { get; set; }
    string Output { get; set; }
    string Error { get; set; }
    string Status { get; set; }
}