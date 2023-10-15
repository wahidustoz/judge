namespace Ilmhub.Judge.Messaging.Shared.Interfaces;

public interface IJudgeCommand : ICommand
{
    int LanguageId { get; set; }
    string SourceCode { get; set; }
    Guid TestCaseId { get; set; }
    long? MaxCpu { get; set; }
    long? MaxMemory { get; set; }
    bool? UseStrictMode { get; set; }
}
