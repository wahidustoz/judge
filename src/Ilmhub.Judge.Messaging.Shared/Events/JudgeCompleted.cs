using System.Text.Json.Serialization;
using Ilmhub.Judge.Messaging.Shared.Interfaces;

namespace Ilmhub.Judge.Messaging.Shared.Events;

public record JudgeCompleted : IJudgeCompleted
{
    public string Status { get; set; }
    [JsonConverter(typeof(AbstractConverter<ICompilationResult, CompilationResult>))]
    public ICompilationResult CompilationResult { get; set; }
    [JsonConverter(typeof(AbstractConverter<IEnumerable<ITestCaseResult>, List<TestCaseResult>>))]
    public IEnumerable<ITestCaseResult> TestCases { get; set; }
    public Guid RequestId { get; set; }
    public Guid SourceId { get; set; }
    public string SourceContext { get; set; }
    public string Source { get; set; }
}