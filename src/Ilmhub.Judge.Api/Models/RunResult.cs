using System.Text.Json.Serialization;

namespace Ilmhub.Judge.Api.Models;

public class RunResult
{
    public string ErrorMessage { get; set; }
    public bool IsSuccess => string.IsNullOrWhiteSpace(ErrorMessage);
    
    [JsonPropertyName("CpuTime")]
    public int CpuTime { get; set; }

    [JsonPropertyName("RealTime")]
    public int RealTime { get; set; }

    [JsonPropertyName("Memory")]
    public long Memory { get; set; }

    [JsonPropertyName("Signal")]
    public int Signal { get; set; }

    [JsonPropertyName("ExitCode")]
    public int ExitCode { get; set; }

    [JsonPropertyName("Error")]
    public int Error { get; set; }

    [JsonPropertyName("Result")]
    public int Result { get; set; }
}