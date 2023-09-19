using System.Text.Json.Serialization;

namespace Ilmhub.Judge.Api.Models;

public class RunResult
{
    public string ErrorMessage { get; set; }
    public bool IsSuccess => string.IsNullOrWhiteSpace(ErrorMessage);
    
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

    [JsonPropertyName("error")]
    public int Error { get; set; }

    [JsonPropertyName("result")]
    public int Result { get; set; }
}