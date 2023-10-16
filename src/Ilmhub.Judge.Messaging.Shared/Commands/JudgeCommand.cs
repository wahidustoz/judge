using Ilmhub.Judge.Messaging.Shared.Interfaces;

namespace Ilmhub.Judge.Messaging.Shared.Commands;

public record JudgeCommand : IJudgeCommand
{
    public int LanguageId { get; set; }
    public string SourceCode { get; set; }
    public Guid TestCaseId { get; set; }
    public long? MaxCpu { get; set; }
    public long? MaxMemory { get; set; }
    public bool? UseStrictMode { get; set; }
    public Guid RequestId { get; set; }
    public Guid SourceId { get; set; }
    public string SourceContext { get; set; }
    public string Source { get; set; }
}
