using System.Text.Json.Serialization;
using Ilmhub.Judge.Wrapper.Abstractions.Models;

namespace Ilmhub.Judge.Wrapper;

public class ExecutionResult : IExecutionResult
{
    [JsonPropertyName("cpu_time")]
    public int CpuTime { get; set; }
    [JsonPropertyName("real_time")]
    public int RealTime { get; set; }
    [JsonPropertyName("memory")]
    public long Memory { get; set; }
    [JsonPropertyName("signal")]
    public int Signal { get; set; }
    [JsonPropertyName("exit_code")]
    public int ExitCode { get; set; }
    [JsonPropertyName("result")]
    public EExecutionResult Status { get; set; }
    [JsonPropertyName("error")]
    public EExecutionError Error { get; set; }
    public string Output { get; set; }
    public string ErrorMessage { get; set; }
}
