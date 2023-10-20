using Ilmhub.Judge.Messaging.Shared.Interfaces;

namespace Ilmhub.Judge.Messaging.Shared.Events;

public record JudgeFailed : IJudgeFailed {
    public string Error { get; set; }
    public Guid RequestId { get; set; }
    public Guid SourceId { get; set; }
    public string SourceContext { get; set; }
    public string Source { get; set; }
}