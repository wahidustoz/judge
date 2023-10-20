using Ilmhub.Judge.Messaging.Shared.Interfaces;

namespace Ilmhub.Judge.Messaging.Shared.Commands;

public record RunCommand : IRunCommand {
    public int LanguageId { get; set; }
    public string SourceCode { get; set; }
    public IEnumerable<string> Inputs { get; set; }
    public long? MaxCpu { get; set; }
    public long? MaxMemory { get; set; }
    public Guid RequestId { get; set; }
    public Guid SourceId { get; set; }
    public string SourceContext { get; set; }
    public string Source { get; set; }
}