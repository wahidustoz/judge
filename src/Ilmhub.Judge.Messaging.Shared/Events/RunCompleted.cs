using System.Text.Json.Serialization;
using Ilmhub.Judge.Messaging.Shared.Interfaces;

namespace Ilmhub.Judge.Messaging.Shared.Events;

public record RunCompleted : IRunCompleted
{
    [JsonConverter(typeof(AbstractConverter<ICompilationResult, CompilationResult>))]
    public ICompilationResult CompilationResult { get; set; }
    [JsonConverter(typeof(AbstractConverter<IEnumerable<IRunResult>, List<RunResult>>))]
    public IEnumerable<IRunResult> Outputs { get; set; }
    public Guid RequestId { get; set; }
    public Guid SourceId { get; set; }
    public string SourceContext { get; set; }
    public string Source { get; set; }
}
