using System.Text.Json.Serialization;
using Ilmhub.Judge.Sdk.Json;

namespace Ilmhub.Judge.Sdk.Dtos;

[JsonConverter(typeof(PingResponseConverter))]
public class PingResponse
{
    [JsonPropertyName("err")]
    public string Err { get; set; }

    [JsonPropertyName("data")]
    public PingResponseData Data { get; set; }

    public string ErrorMessage { get; set; }
    public bool IsSuccess => Err is null && Data is not null;
}

public class PingResponseData
{
    [JsonPropertyName("hostname")]
    public string Hostname { get; set; }

    [JsonPropertyName("cpu")]
    public double Cpu { get; set; }

    [JsonPropertyName("cpu_core")]
    public long CpuCore { get; set; }

    [JsonPropertyName("memory")]
    public double Memory { get; set; }

    [JsonPropertyName("judger_version")]
    public string JudgerVersion { get; set; }

    [JsonPropertyName("action")]
    public string Action { get; set; }
}